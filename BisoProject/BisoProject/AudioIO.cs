using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.CoreAudioApi;

namespace BisoProject
{
    class AudioIO
    {
        MMDevice InputDevice;

        public bool ShortRecordStop = false;

        public delegate void VoiceRecorded(byte[] streamByte, VoiceCaptureType captuertype);
        public event VoiceRecorded OnShortVoiceRecorded;

        public void InitCaptureSound()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            MMDevice[] devicesList = devices.ToArray();
            Console.WriteLine("사용하실 마이크 디바이스를 선택해주세요");
            Console.WriteLine("");
            for (int i = 0; i < devicesList.Length; i++)
            {
                Console.WriteLine(i.ToString() + ". " + devicesList[i]);
            }
            Console.WriteLine("");
            selectInput:
            string devicenum = Console.ReadLine();
            try
            {
                Convert.ToInt16(devicenum);
            }
            catch
            {
                Console.WriteLine(devicenum + "은(는) 정수가 아닙니다");
                goto selectInput;
            }
            if (Convert.ToInt16(devicenum) > devicesList.Length - 1 || 0 > Convert.ToInt16(devicenum))
            {
                Console.WriteLine(devicenum + "은(는) 범위 바깥의 값입니다");
                goto selectInput;
            }
            InputDevice = devicesList[Convert.ToInt16(devicenum)];
            
            Console.WriteLine("");
            Console.WriteLine("마이크 트리거를 사용할까요? (Y/N)");
            Console.WriteLine("");
            SetInputTriggerBool:
            string InputTriggerUse = Console.ReadLine();
            if(InputTriggerUse == "Y" || InputTriggerUse == "y")
            {
                ConsoleKeyInfo keys;
                while (true)
                {
                    if (ShortRecordStop == false)
                        CaptureShortSound();

                    if (Console.KeyAvailable)
                    {
                        keys = Console.ReadKey(true);
                        if (keys.Key == ConsoleKey.Spacebar)
                            Program.StartVoiceCapture(VoiceCaptureType.DefaultCapture);
                    }
                }
            }
            else if (InputTriggerUse == "N" || InputTriggerUse == "n")
            {
                ConsoleKeyInfo keys;
                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        keys = Console.ReadKey(true);
                        if (keys.Key == ConsoleKey.Spacebar)
                            Program.StartVoiceCapture(VoiceCaptureType.DefaultCapture);
                    }
                }
            }
            else
            {
                Console.WriteLine("유효한 값을 적어주세요");
                goto SetInputTriggerBool;
            }
        }

        private void CaptureShortSound()
        {
            WasapiCapture capture = new WasapiCapture(InputDevice);
            MemoryStream memoryStream = new MemoryStream();
            WaveFileWriter writer = new WaveFileWriter(memoryStream, capture.WaveFormat);
            long voiceTime = 0;
            long stopPos = 0;
            int stopcount = 0;

            capture.DataAvailable += (s, a) =>
            {
                writer.Write(a.Buffer, 0, a.BytesRecorded);

                int soundValue = ((int)Math.Round(InputDevice.AudioMeterInformation.MasterPeakValue * 100));
                //Console.WriteLine(soundValue);
                if (soundValue > 20)//장치 소리값이 20보다 크다면
                {
                    if (voiceTime == 0)//보이스타임 없을떄
                    {
                        voiceTime = Convert.ToInt64(writer.Position - writer.WaveFormat.AverageBytesPerSecond * 1.5f);//현재 녹음파일의 스트림 위치의 1.5초뒤부터 자름
                    }
                }
                else if (soundValue < 15)//말이 없다면
                {
                    if (voiceTime != 0)//소리를 받는중이라면
                    {
                        if (stopcount >= 5)
                        {
                            if (stopPos == 0)
                                stopPos = (long)(writer.Position + writer.WaveFormat.AverageBytesPerSecond * 0.3f);//0.3초후 중지 예약
                        }
                        stopcount++;
                    }
                }

                if (voiceTime != 0)//소리를 받는중이라면
                {
                    if (writer.Position > voiceTime + capture.WaveFormat.AverageBytesPerSecond * 4f)//1.5(4-2.5)초뒤에 녹음 중지
                    {
                        capture.StopRecording();
                    }
                    if (stopPos != 0)
                    {
                        if (writer.Position > stopPos)//말이 없는지 0.3초후
                        {
                            capture.StopRecording();
                        }
                    }
                }
                if(ShortRecordStop == true)
                {
                    capture.StopRecording();
                }
            };

            capture.RecordingStopped += (s, a) =>
            {
                if (ShortRecordStop == true)
                {
                    writer.Dispose();
                    capture.Dispose();
                    memoryStream.Dispose();
                }
                else
                {
                    writer.Flush();
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    WaveFileReader WaveFileWriterTrimRead = new WaveFileReader(memoryStream);//자를 파일

                    MemoryStream memoryStreamTrim = new MemoryStream();
                    WaveFileWriter WaveFileWriterTrim = new WaveFileWriter(memoryStreamTrim, capture.WaveFormat);//자르고 받을파일

                    double BytesPerMillisecond = WaveFileWriterTrimRead.WaveFormat.AverageBytesPerSecond / 1000.0;//----------------------------------스타트랑 앤드
                    int start = (int)(WaveFileWriterTrimRead.Position * BytesPerMillisecond) + (int)voiceTime;//말시작한 1초앞은 자르기
                    start -= start % WaveFileWriterTrimRead.WaveFormat.BlockAlign;

                    int end = (int)((WaveFileWriterTrimRead.Position + WaveFileWriterTrimRead.TotalTime.Seconds * 1000f) * BytesPerMillisecond);
                    end -= end % WaveFileWriterTrimRead.WaveFormat.BlockAlign;                                    //----------------------------------

                    TrimWavFile(WaveFileWriterTrimRead, WaveFileWriterTrim, start, end);  //이거슨 싹둑싹둑 메소드

                    WaveFileWriterTrimRead.Dispose();//해제

                    WaveFileWriterTrim.Flush();
                    
                    memoryStreamTrim.Seek(0, SeekOrigin.Begin);
                    MemoryStream ConvertedMemory = WaveFormatConversion(memoryStreamTrim, capture.WaveFormat);

                    byte[] fileData = new byte[ConvertedMemory.Length];
                    ConvertedMemory.Read(fileData, 0, fileData.Length);

                    //ConvertedMemory.Seek(0, SeekOrigin.Begin);
                    //PlaySound(ConvertedMemory);//메모리 스트림으로 읽기
                    OnShortVoiceRecorded?.Invoke(fileData, VoiceCaptureType.ShortTriggerCapture);

                    ConvertedMemory.Dispose();
                    writer.Dispose();

                    capture.Dispose();
                    memoryStream.Dispose();
                    memoryStream = null;
                }
            };

            try
            {
                capture.StartRecording();
            }
            catch 
            {
                writer.Dispose();
                capture.Dispose();
                memoryStream.Dispose();
            }

            while (capture.CaptureState != NAudio.CoreAudioApi.CaptureState.Stopped)
            {
                Thread.Sleep(5000);
            }
        }

        private AudIOData CaptureSound()
        {
            WasapiCapture capture = new WasapiCapture(InputDevice);
            
            MemoryStream memoryStream = new MemoryStream();
            WaveFileWriter writer = new WaveFileWriter(memoryStream, capture.WaveFormat);
            long stopPos = 0;
            int stopcount = 0;
            capture.DataAvailable += (s, a) =>
            {
                writer.Write(a.Buffer, 0, a.BytesRecorded);

                int soundValue = ((int)Math.Round(InputDevice.AudioMeterInformation.MasterPeakValue * 100));
                Console.WriteLine("================" + soundValue);
                if (soundValue > 20)//장치 소리값이 20보다 크다면
                {
                    stopcount = 0;
                }
                else if (soundValue < 15 && writer.Position > capture.WaveFormat.AverageBytesPerSecond * 2f)//말이 없다면
                {
                    if (stopcount >= 2)//말 없음 2프레임 지속
                    {
                        if (stopPos == 0)
                            stopPos = (long)(writer.Position + writer.WaveFormat.AverageBytesPerSecond * 0f);//0초후 중지 예약
                    }
                    stopcount++;
                }

                if (writer.Position > capture.WaveFormat.AverageBytesPerSecond * 6f)//6초뒤에 녹음 중지
                {
                    capture.StopRecording();
                }
                if (stopPos != 0)
                {
                    if (writer.Position > stopPos)//말이 없는지 0.3초후 녹음 중지
                    {
                        capture.StopRecording();
                    }
                }
            };

            byte[] fileData = null;//오디오 바이트

            capture.RecordingStopped += (s, a) =>
            {
                writer.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);
                
                MemoryStream ConvertedMemory = WaveFormatConversion(memoryStream, capture.WaveFormat);

                fileData = new byte[ConvertedMemory.Length];//오다오 바이트 할당
                ConvertedMemory.Read(fileData, 0, fileData.Length);
                
                //ConvertedMemory.Seek(0, SeekOrigin.Begin);
                //PlaySound(ConvertedMemory);

                ConvertedMemory.Dispose();
                capture.Dispose();
                memoryStream.Dispose();
                memoryStream = null;
            };

            try
            {
                capture.StartRecording();
            }
            catch
            {
                writer.Dispose();
                capture.Dispose();
                memoryStream.Dispose();
                return new AudIOData(true);
            }

            while (capture.CaptureState != NAudio.CoreAudioApi.CaptureState.Stopped)
            {
                Thread.Sleep(500);
            }
            return new AudIOData(fileData);//이벤트에서 받은 오디오 반환
        }

        public void PlaySound(MemoryStream memoryStream)
        {
            using (var audioFile = new WaveFileReader(memoryStream))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
                memoryStream.Dispose();
            }
        }

        public void PlayMp3Sound(MemoryStream memoryStream)
        {
            using (var audioFile = new Mp3FileReader(memoryStream))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
                memoryStream.Dispose();
            }
        }

        public void PlayMp3Sound(string FileDir)
        {
            using (var audioFile = new Mp3FileReader(FileDir))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        public AudIOData PlayMp3SoundAndRecordSound(string filedir)
        {
            bool Timeronce = false;
            using (var audioFile = new Mp3FileReader(filedir))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(50);
                    if (Timeronce == false && audioFile.CurrentTime.Milliseconds > 200)
                    {
                        Timeronce = true;
                        return CaptureSound();
                    }
                }
            }
            return new AudIOData(true);
        }

        private void TrimWavFile(WaveFileReader reader, WaveFileWriter writer, int startPos, int endPos)
        {
            endPos = endPos > reader.Length ? (int)reader.Length : endPos;
            startPos = startPos > 0 ? startPos : 0;

            reader.Position = startPos;

            byte[] buffer = new byte[reader.WaveFormat.BlockAlign * 100];
            while (reader.Position < endPos)
            {
                int bytesRequired = (int)(endPos - reader.Position);
                if (bytesRequired > 0)
                {
                    int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
        
        private MemoryStream WaveFormatConversion(MemoryStream memoryStream, WaveFormat waveFormat)
        {
            using (var reader = new WaveFileReader(memoryStream))
            {
                var newFormat = new WaveFormat(16000, 16, 1);
                using (var resampler = new MediaFoundationResampler(reader, newFormat))
                {
                    MemoryStream ConvertStream = new MemoryStream();
                    WaveFileWriter.WriteWavFileToStream(ConvertStream, resampler);
                    return ConvertStream;
                }
            }
        }

        public void ShortCaptureCtrol(bool toggle)
        {
            ShortRecordStop = toggle;
        }
    }

    public struct AudIOData
    {
        public readonly byte[] AudioStreamByte;
        public readonly bool error;

        public AudIOData(byte[] AudioStreamByte)
        {
            this.AudioStreamByte = AudioStreamByte;
            this.error = false;
        }

        public AudIOData(bool error)
        {
            this.AudioStreamByte = null;
            this.error = error;
        }
    }
}
