#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("F04PHR0bAwsdTg8NDQseGg8ADQtobTtzYGp4anpFvgcp+hhnkJoF4xkZQA8eHgILQA0BA0EPHh4CCw0PQV7vrWhmRWhva2tpbGxe79h0791OLS9e7G9MXmNoZ0ToJuiZY29vb04BCE4aBgtOGgYLAE4PHh4CBw0PES/G9pe/pAjySgV/vs3VinVErXFE6CbomWNvb2trbl4MX2VeZ2htO3heemhtO2ptfWMvHh4CC048AQEaKxBxIgU++C/nqhoMZX7tL+ld5O9mRWhva2tpbG94cAYaGh4dVEFBGSe2GPFdegvPGfqnQ2xtb25vzexvamh9bDs9X31ef2htO2pkfWQvHh5pghNX7eU9Tr1Wqt/R9CFkBZFFkgAKTg0BAAoHGgcBAB1OAQhOGx0LTg8ACk4NCxwaBwgHDQ8aBwEATh5oXmFobTtzfW9vkWprXm1vb5Fec2HzU51FJ0Z0ppCg29dgtzByuKVTa25t7G9hbl7sb2Rs7G9vbor/x2caBwgHDQ8aC04MF04PABdOHg8cGnH/tXApPoVrgzAX6kOFWMw5IjuCAgtOJwANQF9IXkpobTtqZX1zLx48CwIHDwANC04BAE4aBgcdTg0LHNl10/0sSnxEqWFz2CPyMA2mJe5521TDmmFgbvxl3094QBq7UmO1DHhjaGdE6CbomWNvb2trbm3sb29uMkJODQscGgcIBw0PGgtOHgECBw0XHgILTjwBARpOLS9ecHljXlheWlwMAgtOHRoPAAoPHApOGgscAx1ODxRe7G8YXmBobTtzYW9vkWpqbWxv3142gjRqXOIG3eFzsAsdkQkwC9Jef2htO2pkfWQvHh4CC04nAA1AX1j3IkMW2YPi9bKdGfWcGLwZXiGvW1xfWl5dWDR5Y11bXlxeV1xfWl5TSAlO5F0EmWPsobCFzUGXPQQ1CvvwFGLKKeU1unhZXaWqYSOgege/SF5KaG07amV9cy8eHgILTi0LHBpe7GrVXuxtzc5tbG9sbG9sXmNoZ2YwXuxvf2htO3NOauxvZl7sb2pe5XfnsJclAptpxUxebIZ2UJY+Z723WBGv6Tu3yffXXCyVtrsf8BDPPHHr7et191MpWZzH9S7gQrrf/ny27G9uaGdE6CbomQ0Ka29e75xeRGgaBgEcBxoXX3heemhtO2ptfWMvHsXNH/wpPTuvwUEv3ZaVjR6jiM0iBwgHDQ8aBwEATi8bGgYBHAcaF18eAgtOLQscGgcIBw0PGgcBAE4vGwnhZtpOmaXCQk4BHthRb17i2S2hxrIQTFukS7u3YbgFusxKTX+Zz8KuDV0ZmVRpQjiFtGFPYLTUHXch211YNF4MX2VeZ2htO2pofWw7PV99p3ccmzNguxEx9ZxLbdQ74SMzY5/Qmh31gLwKYaUXIVq2zFCXFpEFphwPDRoHDQtOHRoPGgsDCwAaHUBeCltNeyV7N3Pd+pmY8vChPtSvNj7hHe8OqHU1Z0H83JYqJp4OVvB7m+56Rb4HKfoYZ5CaBeNALsiZKSMRN8lrZxJ5Ljh/cBq92eVNVSnNuwFKjIW/2R6xYSuPSaSfAxaDidt5eUAuyJkpIxFmMF5xaG07c01qdl54PsTku7SKkr5naVneGxtP");
        private static int[] order = new int[] { 40,30,36,37,13,41,17,30,12,25,35,27,36,51,43,45,29,22,43,39,45,37,38,59,59,47,44,38,42,42,44,47,48,44,56,43,43,40,44,42,58,49,56,49,59,59,51,58,54,57,57,56,57,56,59,55,59,57,59,59,60 };
        private static int key = 110;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
