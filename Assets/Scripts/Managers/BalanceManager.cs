using System.Collections.Generic;
using BloomLines.Assets;
using UnityEngine;

#if CONSOLE
using QFSW.QC;
#endif

namespace BloomLines.Managers
{
    // Все игровый скрипты которые должны каким то образом влиять баланс игры, должны реализовывать этот интерфейс
    public interface IBalanceModifier
    {
        int Priority { get; } // Приоритет записи данных
        void Apply(BalanceData data); // Обновляем баланс
        void OnGetBalanceModifiers(GetBalanceModifiersEvent eventData); // Когда кто то пытается получить все IBalanceModifier через ивент, записываем себя туда
    }

    // Ивент для получения всех модификаторов баланса
    public class GetBalanceModifiersEvent
    {
        public List<IBalanceModifier> Modifiers { get; private set; }

        public GetBalanceModifiersEvent(List<IBalanceModifier> modifiers)
        {
            Modifiers = modifiers;
        }
    }

    public static class BalanceManager // Отвечает за игровой баланс
    {
        public static BalanceData Get() // Получить актуальный игровой баланс учитывая все модификаторы
        {
            var gameModeState = SaveManager.GameModeState;

            if (gameModeState == null)
                return GameAssets.BalanceData;

            var data = GameAssets.BalanceData.Clone(); // Копируем основной баланс чтобы не изменять его
            var modifiers = new List<IBalanceModifier>();
            EventsManager.Publish(new GetBalanceModifiersEvent(modifiers)); // Запрашиваем все подификаторы

            modifiers.Sort((a, b) => a.Priority.CompareTo(b.Priority)); // Сортируем их по приоритету

            foreach (var mod in modifiers) // Применяем все модификаторы
                mod.Apply(data);

            return data;
        }
#if CONSOLE
        [Command("get_balance_info")]
#endif
        private static void GetBalanceInfo()
        {
            var data = Get();
            Debug.Log(data.ToString());
        }
    }
}