#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("lwHAp4t2UNj+1W7AH/+2Fketwoqf9Dkwzdhhaw+AH5v4bdtFodPbXAYRMfSAxUzYpVJjrp5OkVhz8cGUZvkzW8Wcw0nKlHKqpukeQ5BZM2LC+3vGiJbHWxymHBYxDlE3UFQcN03cOak9Q3irSlTwJzpQwyV+6RHFzH793szx+vXWerR6C/H9/f35/P/uZBSNqU5EfDc96CIB8js/CiPZn2NovulbtW/Rxrj5IyFA3m32F7w5c/Vk/oQKcqYiPtoyYJvUgHDXLUUTVbQnPWTMTFvIsNyfmBaKW3Cv6X798/zMfv32/n79/fxj5J8ZGzjH6AV7vw573vsgBG8ahz4lL/HcHx8MvK+gMmyjXHWGPcOU7UZztTK1FeKpuyFyk7K/+/7//fz9");
        private static int[] order = new int[] { 3,11,9,9,12,7,9,9,10,13,13,11,13,13,14 };
        private static int key = 252;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
