using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class DateTimeExtensions
    {

        public static DateTime ParseStringToDate(this DateTime date, string dateString, string format = "dd/MM/yyyy HH:mm")
        {

            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                Debug.Log("Data e ora: " + date);
                return date;
            }
            else
            {
                date = new DateTime();
                Debug.LogError("Formato data/ora non valido.");
                return date;
            }

        }

        public static List<DateTime> GetFullCalendarMonth(this DateTime dateTime)
        {
            int year = dateTime.Year;
            int month = dateTime.Month;

            List<DateTime> calendarDays = new List<DateTime>();

            // 1. Primo giorno del mese richiesto
            DateTime firstOfMonth = new DateTime(year, month, 1);

            // 2. Calcoliamo l'inizio della settimana. 
            // In C# DayOfWeek.Monday è 1, Sunday è 0. 
            // Vogliamo che la settimana inizi sempre di lunedì.
            int daysToSubtract = ((int)firstOfMonth.DayOfWeek + 6) % 7;

            // Se il mese inizia di Lunedì, sottraiamo comunque 7 giorni per includere la settimana precedente
            if (daysToSubtract == 0) daysToSubtract = 7;

            DateTime startDate = firstOfMonth.AddDays(-daysToSubtract);

            // 3. Riempiamo la lista fino a coprire l'intero mese e completare l'ultima settimana
            // Utilizziamo un ciclo che continua finché non abbiamo completato le settimane 
            // e siamo passati al mese successivo.
            DateTime currentDay = startDate;

            // Continuiamo ad aggiungere giorni finché:
            // - Non abbiamo finito il mese corrente
            // - O non abbiamo finito la settimana (arrivando a domenica)
            // Per coprire il caso "mese finisce di domenica", forziamo l'aggiunta della settimana successiva.

            while (currentDay < firstOfMonth.AddMonths(1) || currentDay.DayOfWeek != DayOfWeek.Sunday)
            {
                calendarDays.Add(currentDay);
                currentDay = currentDay.AddDays(1);
            }

            // Aggiungiamo l'ultima domenica
            calendarDays.Add(currentDay);

            // Se l'ultima domenica era proprio la fine del mese, 
            // aggiungiamo un'ulteriore settimana del mese successivo come richiesto
            if (calendarDays[calendarDays.Count - 1] == firstOfMonth.AddMonths(1).AddDays(-1))
            {
                for (int i = 0; i < 7; i++)
                {
                    currentDay = calendarDays[calendarDays.Count - 1].AddDays(1);
                    calendarDays.Add(currentDay);
                }
            }

            return calendarDays;
        }
    }
}
