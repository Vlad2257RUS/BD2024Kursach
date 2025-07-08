--симметричное внутреннее соединение с условием отбора заказчиков по внешнему ключу города
CREATE OR REPLACE FUNCTION public.zapr_customer_city_info(bid_city)
RETURNS TABLE (
    "ID_Заказчика" bigint,
    "Заказчик" character varying(32),
    "ID_города" bigint,
    "Город" character varying(32)
)
AS $$
BEGIN
    RETURN QUERY
    SELECT
        c.id_customer,
        c.name_customer,
        cy.id_city,
        cy.city
    FROM
        public.customer c
    INNER JOIN
        public.city cy ON c.fid_city = cy.id_city
	WHERE cy.id_city = bid_city;
END;
$$ LANGUAGE plpgsql;
--симметричное внутреннее соединение с условием отбора каналов по внешнему ключу канала
CREATE OR REPLACE FUNCTION public.zapr_channel_city_info(bid_city)
RETURNS TABLE (
    "ID_Канала" bigint,
    "Канал" character varying(32),
    "ID_города" bigint,
    "Город" character varying(32)
)
AS $$
BEGIN
    RETURN QUERY
    SELECT
        c.id_channel,
        c.channel_name,
        cy.id_city,
        cy.city
    FROM
        public.channel c
    INNER JOIN
        public.city cy ON c.fid_city = cy.id_city
	WHERE cy.id_city = bid_city;
END;
$$ LANGUAGE plpgsql;

--симметричное внутреннее соединение с условием отбора по дате
CREATE OR REPLACE FUNCTION zapr_orders_date(date_v DATE)--
RETURNS TABLE("Номер заказа" bigint, 
			  "Заказчик" text, 
			  "Название канала" text, 
			  "Дата заказа" date, 
			  "Тип времени" text, 
			  "Продолжительность" bigint)

AS $$
BEGIN
    RETURN QUERY
    SELECT a.number_order, 
		    k.name_customer,
            c.channel_name, 
			a.order_date,
            d.time_type,
			a.duration

          
        FROM public.orders a
		INNER JOIN public.customer k ON a.fid_customer = k.id_customer
        INNER JOIN public.channel c ON a.fid_channel = c.id_channel
		INNER JOIN public.time_type d ON a.fid_time_type = d.id_time_type
		WHERE a.order_date < date_v;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION public.zapr_orders_date_after(date_v DATE)--
RETURNS TABLE("ID заказа" bigint, 
			  "Заказчик" character varying(32), 
			  "Название канала" character varying(32), 
			  "Дата заказа" date, 
			  "Тип времени" character varying(32), 
			  "Продолжительность" bigint)

AS $$
BEGIN
    RETURN QUERY
    SELECT a.id_order, 
		    k.name_customer,
            c.channel_name, 
			a.order_date,
            d.time_type,
			a.duration

          
        FROM public.orders a
		INNER JOIN public.customer k ON a.fid_customer = k.id_customer
        INNER JOIN public.channel c ON a.fid_channel = c.id_channel
		INNER JOIN public.time_type d ON a.fid_time_type = d.id_time_type
		WHERE a.order_date >= date_v;
END;
$$ LANGUAGE plpgsql;

--левое внешнее соединение--
CREATE OR REPLACE FUNCTION public.zapr_orders_left()
RETURNS TABLE("ID заказа" bigint, 
			  "Заказчик" character varying(32), 
			  "Название канала" character varying(32), 
			  "Дата заказа" date, 
			  "Тип времени" character varying(32), 
			  "Продолжительность" bigint,
			  "Телефон" bigint
			 )

AS $$
BEGIN
    RETURN QUERY
    SELECT a.id_order, 
		    k.name_customer,
            c.channel_name, 
			a.order_date,
            d.time_type,
			a.duration,
			k.phone_number
        FROM public.orders a
		LEFT JOIN public.customer k ON a.fid_customer = k.id_customer
        INNER JOIN public.channel c ON a.fid_channel = c.id_channel
		INNER JOIN public.time_type d ON a.fid_time_type = d.id_time_type;	
END;
$$ LANGUAGE plpgsql;

--правое внешнее соединение--
CREATE OR REPLACE FUNCTION public.zapr_orders_right()
RETURNS TABLE("ID заказа" bigint, 
			  "Заказчик" character varying(32), 
			  "Название канала" character varying(32), 
			  "Дата заказа" date, 
			  "Тип времени" character varying(32), 
			  "Продолжительность" bigint,
			  "Адрес" character varying(32)
			 )
AS $$
BEGIN
    RETURN QUERY
    SELECT a.id_order, 
		    k.name_customer,
            c.channel_name, 
			a.order_date,
            d.time_type,
			a.duration,
			k.address
          
        FROM public.orders a
		INNER JOIN public.customer k ON a.fid_customer = k.id_customer
        INNER JOIN public.channel c ON a.fid_channel = c.id_channel
		RIGHT JOIN public.time_type d ON a.fid_time_type = d.id_time_type;		
END;
$$ LANGUAGE plpgsql;

--запрос на запросе по принципу левого ссоединения--
CREATE OR REPLACE FUNCTION public.zapr_orders_request_on_r_l()--
 RETURNS TABLE("ID заказа" bigint, 
			   "Заказчик" character varying, 
			   "Название канала" character varying, 
			   "Дата заказа" date, 
			   "Тип времени" character varying, 
			   "Продолжительность" bigint, 
			   "ID возможного заказа" bigint, 
			   "Сотрудник" character varying)
AS $$
BEGIN
    RETURN QUERY
        SELECT a.id_order, 
		    k.name_customer,
            c.channel_name, 
			a.order_date,
            d.time_type,
			a.duration,
			j.id_possible_orders,
			i.employee
        FROM public.orders a
		LEFT JOIN public.customer k ON a.fid_customer = k.id_customer
        LEFT JOIN public.channel c ON a.fid_channel = c.id_channel
		LEFT JOIN public.time_type d ON a.fid_time_type = d.id_time_type
		JOIN public.possible_orders j ON j.id_possible_orders = a.fid_possible_order
		JOIN public.employee i ON i.id_employee = a.fid_employee
		WHERE a.duration > 10;		
END;
$$ LANGUAGE plpgsql;

--симметричное внутреннее соединение без условия (три запроса)--
CREATE OR REPLACE FUNCTION public.get_channel_inner1()
    RETURNS TABLE("ID" bigint, 
                  "Название канала" character varying(32), 
                  "Город" character varying(32), 
                  "Тип собственности"character varying(32), 
                  "Время начала работы" time without time zone, 
                  "Время окончания работы" time without time zone)
    
AS $$
BEGIN
    RETURN QUERY 
        SELECT 
            ch.id_channel, 
            ch.channel_name, 
            ci.city,  
            t.type_of_ownership, 
            ch.start_work, 
            ch.end_work
        FROM public.channel ch
        INNER JOIN public.city ci ON ch.fid_city = ci.id_city
        INNER JOIN public.type_of_ownership t ON ch.fid_type_of_ownership = t.id_type_of_ownership;
END;
$$ LANGUAGE 'plpgsql';

CREATE OR REPLACE FUNCTION get_customer_inner2()
RETURNS TABLE
(
    "ID" bigint, 
    "Наименование заказчика" character varying(32), 
    "Город" character varying(32), 
    "Телефон" bigint

) AS $$
BEGIN
    RETURN QUERY 
        SELECT a.id_customer, 
            a.name_customer, 
            c.city, 
            a.phone_number

        FROM public.customer a
        INNER JOIN public.city c ON a.fid_city = c.id_city;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION public.get_employee_inner3()
    RETURNS TABLE("ID" bigint, 
				  "Сотрудник" character varying(32), 
				  "Роль" character varying(32))

AS $$
BEGIN
    RETURN QUERY 
        SELECT a.id_employee,
			a.employee,
			e.role
            
        FROM public.employee a
		INNER JOIN public.role e ON a.fid_role = e.id_role;
END;
$$ LANGUAGE 'plpgsql';


--итоговый запрос без условия c итоговыми данными вида: «всего»
CREATE OR REPLACE VIEW zapr_vsego AS
SELECT COUNT(*) AS количество_заказов, AVG(a.duration) 
AS средняя_продолжительность FROM public.orders a


--итоговый запрос без условия c итоговыми данными вида: «в том числе»
CREATE OR REPLACE VIEW zapr_v_tom_chisle AS
SELECT
    ch.channel_name AS Наименование_канала,
    COUNT(o.id_order) AS Количество_Заказов,
    AVG(o.duration) AS Средняя_Продолжительность
FROM
    public.channel ch
RIGHT JOIN
    public.orders o ON ch.id_channel = o.fid_channel
GROUP BY
    ch.channel_name;



итоговый запрос с условием на данные по маске – 
вывод всех заказчиков и заказы, которые они заказали, по наименованию заказчика 
CREATE OR REPLACE VIEW zapr_mask2 AS
SELECT a.id_customer, 
       a.name_customer AS Заказчик, 
       c.city AS Город, 
       a.phone_number AS Телефон, 
       a.address AS Адрес
FROM public.customer a
JOIN public.city c ON c.id_city = a.fid_city
WHERE CAST(a.phone_number AS TEXT) LIKE '7%'; -- Пример условия маскирования для номеров телефонов, начинающихся с '7'


--6 лаба

-- итоговый запрос без условия; --
CREATE OR REPLACE VIEW public.zapr_bez_uslov_city
    AS
     SELECT e.id_employee,
    c.channel_name,
    e.employee,
    e.fid_role,
    
   FROM employee e
     JOIN channel c ON e.fid_channel = c.id_channel;

 --итоговый запрос с условием на данные; --по значению--
 CREATE VIEW public.zapr_uslov_dannie
 AS
SELECT a.id_order, 
		    k.name_customer,
            c.channel_name, 
			a.order_date,
            d.time_type,
			a.duration,
          
        FROM public.orders a
		LEFT JOIN public.customer k ON a.fid_customer = k.id_customer
        LEFT JOIN public.channel c ON a.fid_channel = c.id_channel
		LEFT JOIN public.time_type d ON a.fid_time_type = d.id_time_type
		WHERE a.duration = 6;

ALTER TABLE public.zapr_uslov_dannie
    OWNER TO postgres;
 
--по маске
CREATE OR REPLACE VIEW public.maska--
SELECT 
    a.id_order, 
    k.name_customer,
    c.channel_name
FROM 
    public.orders a
INNER JOIN 
    public.customer k ON a.fid_customer = k.id_customer
INNER JOIN 
    public.channel c ON a.fid_channel = c.id_channel
WHERE 
    k.name_customer = 'Имя клиента'  -- Фильтруем по имени клиента
GROUP BY 
    k.name_customer, a.id_order  -- Группируем по имени клиента и id_order
ORDER BY 
    c.channel_name ASC;  -- Сортируем по каналу в порядке возрастания
 

 --итоговый запрос с условием на данные;    с использованием индекса
 --итоговый запрос с условием на данные;    без использования индекса
 
 
 
 
 
-- итоговый запрос с условием на группы;--
/////Ошибка
CREATE VIEW public.zapr_on_grup
 AS
SELECT 
    a.id_order, 
    k.name_customer,
    c.channel_name, 
    a.order_date,
    d.time_type,
    a.duration,
    j.id_possible_orders,

FROM public.orders a
JOIN public.customer k ON k.id_customer = a.fid_customer
JOIN public.channel c ON c.id_channel = a.fid_channel
JOIN public.time_type d ON d.id_time_type = a.fid_time_type
JOIN public.possible_orders j ON j.id_possible_orders = a.fid_possible_order

WHERE  d.time_type = 'Утренний' -- Условие на имя сотрудника
GROUP BY 
    a.order_date; -- Сортировка по дате;
////////////////////////////////////////////////////////////////
-- итоговый запрос с условием на группы;--
CREATE VIEW public.zapr_on_grup
 AS	
	SELECT 
    a.order_date,
	k.name_customer,
    COUNT(a.id_order) AS количество_заказов 
FROM public.orders a
LEFT JOIN public.customer k ON a.fid_customer = k.id_customer
JOIN public.channel c ON c.id_channel = a.fid_channel
JOIN public.time_type d ON d.id_time_type = a.fid_time_type
JOIN public.possible_orders j ON j.id_possible_orders = a.fid_possible_order
WHERE k.name_customer = 'Виктор' -- Условие на имя сотрудника
GROUP BY a.order_date,k.name_customer
ORDER BY a.order_date;

ALTER TABLE public.zapr_on_grup
    OWNER TO postgres;

-- итоговый запрос с условием на данные и на группы;--
///////Ошибка
SELECT 
    a.id_order, 
    k.name_customer,
    c.channel_name, 
    a.order_date,
    d.time_type,
    a.duration
FROM public.orders a
JOIN public.customer k ON k.id_customer = a.fid_customer
JOIN public.channel c ON c.id_channel = a.fid_channel
JOIN public.time_type d ON d.id_time_type = a.fid_time_type
JOIN public.possible_orders j ON j.id_possible_orders = a.fid_possible_order

WHERE c.channel_name = 'Первый' -- Условие на канал
    AND a.duration >= 8 -- Условие на длительность
GROUP BY a.order_date; -- Сортировка по дате
/////////////
-- итоговый запрос с условием на данные и на группы;--
CREATE OR REPLACE VIEW public.zapr_dan_grop
SELECT 
    a.order_date AS дата_заказа,
    COUNT(a.id_order) AS количество_заказов
FROM public.orders a
JOIN public.channel c ON c.id_channel = a.fid_channel
WHERE c.channel_name = 'Первый' AND a.duration >= 8
GROUP BY a.order_date
ORDER BY a.order_date;

 --запрос на запросе по принципу итогового запроса;--
 WITH orders_count AS (
SELECT fid_customer, 
COUNT(id_order) AS num_orders
    FROM orders  GROUP BY fid_customer
)
SELECT c.id_customer, c.name_customer, o.num_orders, 
       ROUND((o.num_orders / SUM(o.num_orders) OVER()) * 100, 2) AS percentage
FROM customer c
JOIN orders_count o ON c.id_customer = o.fid_customer
ORDER BY o.num_orders DESC;
 



-- запрос с подзапросом с операциями над итоговыми данными--
CREATE OR REPLACE VIEW public.zapr_itog
        SELECT a.id_order, 
		    k.name_customer,
            c.channel_name, 
			a.order_date,
            d.time_type,
			a.duration
          
        FROM public.orders a
		JOIN public.customer k ON k.id_customer = a.fid_customer
        JOIN public.channel c ON c.id_channel = a.fid_channel
		JOIN public.time_type d ON d.id_time_type = a.fid_time_type
		
		WHERE a.duration < ( SELECT AVG(a.duration)
		FROM public.orders a);

-- запрос с подзапросом in с операциями над итоговыми данными
CREATE OR REPLACE FUNCTION zapr_in()
RETURNS TABLE(
    "Номер заказа" bigint,
    "Дата заказа" DATE,
    "Продолжительность" bigint,
    "Заказчик" text
) AS $$
BEGIN 
    RETURN QUERY SELECT 
        a.number_order, 
        a.order_date,
        a.duration,
        k.name_customer
    FROM 
        public.orders a
        RIGHT JOIN public.customer k ON a.fid_customer = k.id_customer
    WHERE 
        a.duration IN (SELECT AVG(duration) FROM public.orders) AND a.order_date >= '2020-10-01'
        GROUP BY a.order_date
    ORDER BY a.order_date;

END; 
$$ LANGUAGE plpgsql;


-- запрос с подзапросом not in с операциями над итоговыми данными
--duration не равно среднему значению duration для всех заказов.
CREATE OR REPLACE FUNCTION zapr_notin()
RETURNS TABLE(
    number_order INTEGER,
    order_date DATE,
    duration INTEGER
) AS $$
BEGIN 
    RETURN QUERY SELECT 
    a.id_order,  
    a.order_date,
    a.duration

FROM 
    public.orders a
JOIN 
    public.customer k ON k.id_customer = a.fid_customer
JOIN 
    public.channel c ON c.id_channel = a.fid_channel
JOIN 
    public.time_type d ON d.id_time_type = a.fid_time_type

WHERE 
    a.duration NOT IN (SELECT AVG(duration) FROM public.orders);
    END; 
$$ LANGUAGE plpgsql;

--Этот запрос вернет набор данных с информацией о заказах. Для каждого заказа будет добавлен 
--столбец duration_status, который будет содержать:'Среднее значение', 'Не среднее значение'
CREATE OR REPLACE VIEW zapr_case AS
SELECT a.number_order AS Номер_заказа, 
       k.name_customer AS Наименование_заказчика, 
       a.order_date AS Дата_заказа,
       a.duration AS Продолжительность,
       CASE
           WHEN a.duration = (SELECT AVG(duration) FROM public.orders) THEN 'Среднее значение'  
           ELSE 'Не среднее значение'                           
       END AS duration_status  -- Создание нового столбца для статуса
FROM public.orders a
INNER JOIN public.customer k ON k.id_customer = a.fid_customer;



Этот запрос вернет набор данных с информацией о заказах, где значение duration 
не равно среднему значению duration для всех заказов в группе.
CREATE OR REPLACE VIEW zapr_having AS
SELECT a.number_order AS Номер_заказа, 
    k.name_customer AS Наименование_заказчика, 
    a.order_date AS Дата_заказа,
    a.duration AS Продолжительность

FROM 
public.orders a
JOIN public.customer k ON k.id_customer = a.fid_customer
GROUP BY a.id_order, k.name_customer,c.channel_name, 
    a.order_date,d.time_type,a.duration
HAVING a.duration <> AVG(a.duration);  -- Сравнение с AVG
ORDER BY a.order_date; -- Сортировка по дате

----запрос с использованием объединения	
CREATE OR REPLACE VIEW zapr_union	AS
SELECT name_city AS название FROM city
UNION
SELECT name_type_of_ownership AS название FROM type_of_ownership;


CREATE INDEX idx_orders_order_date ON orders (order_date);
--с использованием индекса
CREATE OR REPLACE VIEW public.zapr_index
SELECT 
    a.id_order, 
    k.name_customer,
    c.channel_name, 
    a.order_date,
    d.time_type,
    a.duration

FROM 
    public.orders a
JOIN 
    public.customer k ON k.id_customer = a.fid_customer
JOIN 
    public.channel c ON c.id_channel = a.fid_channel
JOIN 
    public.time_type d ON d.id_time_type = a.fid_time_type
WHERE 
    a.order_date >= '2023-10-01'  -- Пример условия фильтрации по дате
ORDER BY 
    a.order_date; -- Сортировка по дате


--Exel
CREATE OR REPLACE FUNCTION get_top_cities_with_most_customer()
RETURNS TABLE 
(
    "Город" TEXT,
    "Количество заказчиков" INTEGER
) AS $$
BEGIN
    RETURN QUERY
    SELECT c.city::TEXT,
           COUNT(*)::INTEGER AS "Количество заказчиков"
    FROM public.customer AS cust
    JOIN public.city AS c ON c.id_city = cust.fid_city
    GROUP BY c.city
    ORDER BY COUNT(*) DESC
    LIMIT 5;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION get_orders_with_longest_duration()
RETURNS TABLE(
    "ID" bigint, 
    "Дата заказа" date,
    "Продолжительность" bigint,
    "Номер заказа" bigint
) AS $$
BEGIN
    RETURN QUERY
    SELECT id_order::INTEGER,  order_date::DATE, duration::INTEGER, number_order::INTEGER
    FROM public.orders
    ORDER BY duration DESC
    LIMIT 5;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION get_channels_with_most_possible_orders()
RETURNS TABLE(
    "ID" bigint,
    "Канал" text,
    "Количество возможных заказов" bigint
) AS $$
BEGIN
    RETURN QUERY
    SELECT c.id_channel::INTEGER, c.channel_name::TEXT, SUM(po.number_possible_orders)::INTEGER as total_possible_orders
    FROM public.channel c
    JOIN public.possible_orders po ON c.id_channel = po.fid_channel
    GROUP BY c.id_channel
    ORDER BY total_possible_orders DESC
    LIMIT 5;
END;
$$ LANGUAGE plpgsql;