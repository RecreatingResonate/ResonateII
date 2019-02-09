using System;
using System.Runtime.InteropServices;

namespace ResonateII
{
    [Flags]
    public enum BASSMOD_BassMusic
    {
        BASS_MUSIC_RAMP = 1,
        BASS_MUSIC_LOOP = 4,
        BASS_MUSIC_SURROUND2 = 1024
    }
    public enum BASSMOD_BASSInit
    {
        BASS_DEVICE_DEFAULT = 0
    }

    public static class FlanMOD
    {
        private const string BassModLib = "BASSMOD.dll";
        [DllImport(BassModLib, EntryPoint = "BASSMOD_Init")]
        public static extern IntPtr BASSMOD_Init(int device, int freq, BASSMOD_BASSInit flag);
        [DllImport(BassModLib, EntryPoint = "BASSMOD_MusicLoad")]
        public static extern IntPtr BASSMOD_MusicLoad(bool mem, byte[] tune, int offset, int len, BASSMOD_BassMusic flag);
        [DllImport(BassModLib, EntryPoint = "BASSMOD_Free")]
        public static extern IntPtr BASSMOD_Free();
        [DllImport(BassModLib, EntryPoint = "BASSMOD_MusicPlay")]
        public static extern bool BASSMOD_MusicPlay();
    }
}