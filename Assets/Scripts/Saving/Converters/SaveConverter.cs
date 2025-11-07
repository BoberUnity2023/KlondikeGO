using System.Collections.Generic;
using UnityEngine;

namespace BloomLines.Saving.Convertors
{
    // Класс которые отвечает за конвертацию сохранений игры с одной версии на другую
    public static class SaveConverter
    {
        private static List<ISaveConverter> Convertors;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Convertors = new List<ISaveConverter>();

            // Добавляем нужны конверторы

            //Convertors.Add(new SaveConverter_To_1_2_0());
        }

        // Конвертировать сохранение с нужной версию на нужную
        public static string Convert<T>(string json, string fromVersion, string toVersion) where T : SaveState
        {
            if (string.IsNullOrEmpty(json) || string.IsNullOrWhiteSpace(json) || fromVersion == toVersion)
                return json;

            if (TryGetStartAndEndConvertor(fromVersion, toVersion, out int startConvertor, out int endConvertor))
            {
                for (int i = startConvertor; i <= endConvertor; i++)
                {
                    Debug.Log($"Convert {typeof(T).Name} Save to version: " + Convertors[i].GetVersion());
                    json = Convertors[i].Convert<T>(json);
                }
            }

            return json;
        }

        // Получить конвертор с нужной версии на нужную версию
        private static bool TryGetStartAndEndConvertor(string fromVersion, string toVersion, out int startConvertor, out int endConvertor)
        {
            GetStartAndEndConvertor(fromVersion, toVersion, out startConvertor, out endConvertor);

            if (endConvertor == -1 || startConvertor == -1)
            {
                Debug.LogWarning($"Missing SaveConvertor: {fromVersion} : {toVersion}");
                return false;
            }

            return true;
        }
        
        // Получить конвертор с нужной версии на нужную версию
        private static void GetStartAndEndConvertor(string fromVersion, string toVersion, out int startConvertor, out int endConvertor)
        {
            startConvertor = 0;
            endConvertor = -1;

            if (!string.IsNullOrEmpty(fromVersion) && !string.IsNullOrWhiteSpace(fromVersion))
            {
                startConvertor = -1;

                for(int i = 0; i < Convertors.Count; i++)
                {
                    if (Convertors[i].GetVersion() == fromVersion)
                    {
                        startConvertor = i + 1;
                        break;
                    }
                }
            }

            var toVersionC = new System.Version(toVersion);

            for (int i = 0; i < Convertors.Count; i++)
            {
                var convertorVersion = new System.Version(Convertors[i].GetVersion());

                if (convertorVersion <= toVersionC)
                    endConvertor = i;
            }
        }
    }
}