﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OliverTwist.FilterContainers
{
    public class CitiesDropdown
    {
        private static readonly List<string> _cityes = new List<string>
            (
                new[]
              {
                "Азов (Ростовская область)", 
                "Александров (Владимирская область)", 
                "Анапа (Краснодарский край)", 
                "Арзамас (Нижегородская область)", 
                "Архангельск (Архангельская область)", 
                "Астрахань (Астраханская область)", 
                "Белгород (Белгородская область)", 
                "Белев (Тульская область)", 
                "Белозерск (Вологодская область)", 
                "Болгар (Республика Татарстан)", 
                "Борисоглебский (Ярославская область)", 
                "Боровск (Калужская область)", 
                "Бородино (Московская область)", 
                "Брянск (Брянская область)", 
                "Валдай (Новгородская область)", 
                "Великие Луки (Псковская область)", 
                "Великий Новгород (Новгородская область)", 
                "Великий Устюг (Вологодская область)", 
                "Венёв (Тульская область)", 
                "Верхотурье (Свердловская область)", 
                "Владимир (Владимирская область)", 
                "Волгоград (Волгоградская область)", 
                "Вологда (Вологодская область)", 
                "Воробьи (Калужская область)", 
                "Воронеж (Воронежская область)", 
                "Выборг (Ленинградская область)", 
                "Вытегра (Вологодская область)", 
                "Вышний Волочек (Тверская область)", 
                "Вязники (Владимирская область)", 
                "Вязьма (Смоленская область)", 
                "Гатчина (Ленинградская область)", 
                "Геленджик (Краснодарский край)", 
                "Городец (Нижегородская область)", 
                "Гороховец (Владимирская область)", 
                "Гусь-Хрустальный (Владимирская область)", 
                "Демидов (Смоленская область)", 
                "Дивеево (Нижегородская область)", 
                "Дивногорье (Воронежская область)", 
                "Дмитров (Московская область)", 
                "Дубна (Московская область)", 
                "Егорьевск (Московская область)", 
                "Екатеринбург (Свердловская область)", 
                "Елабуга (Республика Татарстан)", 
                "Елец (Липецкая область)", 
                "Ессентуки (Ставропольский край)", 
                "Железноводск (Ставропольский край)", 
                "Жуковский (Московская область)", 
                "Задонск (Липецкая область)", 
                "Заокский (Тульская область)", 
                "Зарайск (Московская область)", 
                "Звенигород (Московская область)", 
                "Златоуст (Челябинская область)", 
                "Ивангород (Ленинградская область)", 
                "Иваново (Ивановская область)", 
                "Ижевск (Республика Удмуртия)", 
                "Изборск (Псковская область)", 
                "Йошкар-Ола (Республика Марий Эл)", 
                "Казань (Республика Татарстан)", 
                "Калининград (Калининградская область)", 
                "Калуга (Калужская область)", 
                "Калязин (Тверская область)", 
                "Камышин (Волгоградская область)", 
                "Каргополь (Архангельская область)", 
                "Касимов (Рязанская область)", 
                "Кашин (Тверская область)", 
                "Кемерово (Кемеровская область)", 
                "Кемь (Республика Карелия)", 
                "Кимры (Тверская область)", 
                "Кинешма (Ивановская область)", 
                "Кириллов (Вологодская область)", 
                "Киров (Кировская область)", 
                "Кисловодск (Ставропольский край)", 
                "Клин (Московская область)", 
                "Коломна (Московская область)", 
                "Кондопога (Республика Карелия)", 
                "Кострома (Костромская область)", 
                "Краснодар (Краснодарский край)", 
                "Кубинка-1 (Московская область)", 
                "Кунгур (Пермский край)", 
                "Курск (Курская область)", 
                "Липецк (Липецкая область)", 
                "Магнитогорск (Челябинская область)", 
                "Мичуринск (Тамбовская область)", 
                "Можайск (Московская область)", 
                "Москва (Московская область)", 
                "Мурманск (Мурманская область)", 
                "Муром (Владимирская область)", 
                "Мышкин (Ярославская область)", 
                "Невьянск (Свердловская область)", 
                "Нижний Новгород (Нижегородская область)", 
                "Нижний Тагил (Свердловская область)", 
                "Новозыбков (Брянская область)", 
                "Новороссийск (Краснодарский край)", 
                "Новосибирск (Новосибирская область)", 
                "Новочеркасск (Ростовская область)", 
                "Омск (Омская область)", 
                "Орел (Орловская область)", 
                "Оренбург (Оренбургская область)", 
                "Осташков (Тверская область)", 
                "Павлово на Оке (Нижегородская область)", 
                "Павловский Посад (Московская область)", 
                "Пенза (Пензенская область)", 
                "Переделкино (Московская область)", 
                "Переславль-Залесский (Ярославская область)", 
                "Пермь (Пермский край)", 
                "Петергоф (Ленинградская область)", 
                "Петрозаводск (Республика Карелия)", 
                "Печоры (Псковская область)", 
                "Плес (Ивановская область)", 
                "Подольск (Московская область)", 
                "Покров (Владимирская область)", 
                "Порхов (Псковская область)", 
                "Приозерск (Ленинградская область)", 
                "Псков (Псковская область)", 
                "Пудож (Республика Карелия)", 
                "Пушкин (Ленинградская область)", 
                "Пушкинские горы (Псковская область)", 
                "Пятигорск (Ставропольский край)", 
                "Раменское (Московская область)", 
                "Ростов Великий (Ярославская область)", 
                "Ростов-на-Дону (Ростовская область)", 
                "Рускеала (Республика Карелия)", 
                "Рыбинск (Ярославская область)", 
                "Рязань (Рязанская область)", 
                "Самара (Самарская область)", 
                "Санкт-Петербург (Ленинградская область)", 
                "Саранск (Республика Мордовия)", 
                "Саратов (Саратовская область)", 
                "Свияжск (Республика Татарстан)", 
                "Сергиев Посад (Московская область)", 
                "Серпухов (Московская область)", 
                "Смоленск (Смоленская область)", 
                "Соликамск (Пермский край)", 
                "Соловки (Архангельская область)", 
                "Солотча (Рязанская область)", 
                "Сортавала (Республика Карелия)", 
                "Старая Русса (Новгородская область)", 
                "Старица (Тверская область)", 
                "Старый Оскол (Белгородская область)", 
                "Судогда (Владимирская область)", 
                "Суздаль (Владимирская область)", 
                "Сыктывкар (Республика Коми)", 
                "Таганрог (Ростовская область)", 
                "Тамань (Краснодарский край)", 
                "Тамбов (Тамбовская область)", 
                "Таруса (Калужская область)", 
                "Тверь (Тверская область)", 
                "Тихвин (Ленинградская область)", 
                "Тобольск (Тюменская область)", 
                "Тольятти (Самарская область)", 
                "Томск (Томская область)", 
                "Торжок (Тверская область)", 
                "Торопец (Тверская область)", 
                "Тотьма (Вологодская область)", 
                "Тула (Тульская область)", 
                "Тутаев (Ярославская область)", 
                "Тюмень (Тюменская область)", 
                "Углич (Ярославская область)", 
                "Ульяновск (Ульяновская область)", 
                "Уфа (Республика Башкортостан)", 
                "Ферапонтово (Вологодская область)", 
                "Хмелита (Смоленская область)", 
                "Чебоксары (Республика Чувашия)", 
                "Челябинск (Челябинская область)", 
                "Череповец (Вологодская область)", 
                "Шуя (Ивановская область)", 
                "Элиста (Республика Калмыкия)", 
                "Эльбрус (Республика Кабардино-Балкария)", 
                "Юрьев-Польский (Владимирская область)", 
                "Ярославль (Ярославская область)", 
                "Ясная Поляна (Тульская область)"
              }
            );
        public static List<SelectListItem> Get(string selectedCity)
        {
            return _cityes.Select
                (
                    item_city =>
                    new SelectListItem
                                {
                                    Text = item_city,
                                    Value = item_city,
                                    Selected = item_city == selectedCity
                                }
                ).ToList();
        }
    }
}