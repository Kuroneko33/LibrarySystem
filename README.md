# LibrarySystem
Тема: система учета выдачи книг в библиотеке

Описание информационной системы:

	Администратор данной системы должен вести учет книжного фонда библиотеки.
	В его функции входит: 
		управление пользователями системы:
			создание 
			удаление 
			редактирование 
		управление книжным фондом:
			ввод данных о поступающих книгах
			удаление данных о списанных книгах

	Каждый пользователь характеризуется: 
		ФИО 
		пароль доступа 

	Каждая книга характеризуется: 
		ФИО автора
		название
		издательство
		год издания
		количество страниц
		месторасположение

	Пользователем системы является библиотекарь, который может: 
		создавать записи абонементов библиотеки
		осуществлять регистрацию: 
			выдачи 
			возврата 
		книг в библиотеку на абонемент. 

	Абонемент характеризуется следующими полями: 
		ФИО
		паспортные данные, допустим:
			страна
		адрес
		контактный телефон 

	Акт выдачи или возврата книги описывается:
		датой
		абонементом
		книгой
		пользователем(библиотекарем), осуществившим эту запись.

	Дополнительно система должна предоставлять: 
		отчет о выдаче определенной книги 
		отчет по определенному абонементу 

	Доступ администратора и	пользователей к системе осуществляется после процедуры аутентификации. 
	Ввод данных о выдаче и возврате книг должен осуществляться с авторизацией. (будет проходить вместе с аутентификацией)
