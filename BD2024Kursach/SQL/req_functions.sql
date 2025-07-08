----Operation for tables function----
--SELECT data functions
CREATE OR REPLACE FUNCTION get_city_info()
RETURNS TABLE
(
    "ID" bigint,
    "Город" text
) AS $$
BEGIN
    RETURN QUERY SELECT a.id_city, a.city FROM public.city a;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION get_type_of_ownership_info()
RETURNS TABLE
(
    "ID" bigint,
    "Тип собственности" text
) AS $$
BEGIN
    RETURN QUERY 
        SELECT a.id_type_of_ownership, a.type_of_ownership 
        FROM public.type_of_ownership a;
END;
$$ LANGUAGE plpgsql;


CREATE FUNCTION get_role_info()
RETURNS TABLE (
    "ID" bigint,
    "Тип роли" text
)
AS $$
BEGIN
	RETURN QUERY SELECT a.id_role, a.role FROM public.role a;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION get_channel_info()
    RETURNS TABLE("ID" bigint, 
    "Название канала" text, 
    "Город" text, 
    "Тип собственности" text, 
    "Время начала работы" time without time zone, 
    "Время окончания работы" time without time zone)
   
AS $$
BEGIN
    RETURN QUERY 
        SELECT a.id_channel, 
            a.channel_name, 
            c.city,  
            d.type_of_ownership, 
            a.start_work, 
            a.end_work
        FROM public.channel a
        JOIN public.city c ON c.id_city = a.fid_city
        JOIN public.type_of_ownership d ON d.id_type_of_ownership = a.fid_type_of_ownership;
END;
$$ LANGUAGE 'plpgsql';

CREATE OR REPLACE FUNCTION get_customer_info()
RETURNS TABLE
(
    "ID" bigint, 
    "Наименование заказчика" text, 
    "Город" text, 
    "Телефон" bigint,
	"Адрес" text
) AS $$
BEGIN
    RETURN QUERY 
        SELECT a.id_customer, 
            a.name_customer, 
            c.city, 
            a.phone_number, 
            a.address
        FROM public.customer a
        JOIN public.city c ON c.id_city = a.fid_city;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION get_time_type_info()
    RETURNS TABLE
	(
		"ID" bigint, 
		"Тип времени" text, 
		"Количество минут" bigint, 
		"Цена за минуту" bigint 
		) 
AS $$
BEGIN
    RETURN QUERY 
        SELECT a.id_time_type, 
            a.time_type, 
            a.number_of_minutes, 
            a.cost_per_minute
        FROM public.time_type a;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION get_orders_info()
    RETURNS TABLE
	(
        "ID" bigint, 
		"Заказчик" text, 
		"Название канала" text,
		"Дата заказа" date,
		"Тип времени" text,
		"Продолжительность" bigint,
		"Номер возможного заказа" bigint,
        "Номер заказа" bigint
		--"Сотрудник" character varying(32)
		
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
			j. number_possible_orders,
            a.number_order
			--i.employee
          
        FROM public.orders a
		JOIN public.customer k ON k.id_customer = a.fid_customer
        JOIN public.channel c ON c.id_channel = a.fid_channel
		JOIN public.time_type d ON d.id_time_type = a.fid_time_type
		JOIN public.possible_orders j ON j.id_possible_orders = a.fid_possible_order
		JOIN public.employee i ON i.id_employee = a.fid_employee;

END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION get_employee_info()
    RETURNS TABLE("ID" text, 
				  "Сотрудник" text,
				--  "Роль" text,
				  "Логин" text,
				--  "Пароль" text
				  "Прочее" text
				  )

AS $$
BEGIN
    RETURN QUERY 
        SELECT login::TEXT,
			employee::TEXT,
			login::TEXT,
             data::TEXT
        FROM public.employee;

END;
$$ LANGUAGE 'plpgsql';



CREATE OR REPLACE FUNCTION get_possible_orders_info()
    RETURNS TABLE("ID" bigint, 
    "Номер возможного заказа" bigint, 
    "Название канала" text, 
    "Тип времени" text)

AS $$
BEGIN
    RETURN QUERY 
        SELECT a.id_possible_orders, 
            a.number_possible_orders,
            c.channel_name, 
            d.time_type 
          
        FROM public.possible_orders a
		JOIN public.channel c ON c.id_channel = a.fid_channel
        JOIN public.time_type d ON d.id_time_type = a.fid_time_type;

END;
$$  LANGUAGE 'plpgsql';



CREATE OR REPLACE FUNCTION private_get_emp_channel_data()
RETURNS TABLE 
(
    id_channel bigint,
    channel_name text,
    fid_city bigint,
    fid_type_of_ownership bigint,
    start_work character varying(5),
    end_work character varying(5)
) AS $$
BEGIN
    RETURN QUERY SELECT a.id_channel, a.channel_name, a.fid_city, a.fid_type_of_ownership, a. start_work, a.end_work
        FROM channel a
        WHERE a.fid_employee = CURRENT_USER;
END;
$$ LANGUAGE plpgsql;

--
--INSERT data functions--
--

CREATE OR REPLACE FUNCTION insert_data_city
(
    city text
)
RETURNS VOID AS $$
BEGIN
    INSERT INTO city(city) VALUES (city);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION insert_data_role
(
    role text
)
RETURNS VOID AS $$
BEGIN
    INSERT INTO role(role) VALUES (role);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION insert_data_type_of_ownership
(
    type_name text
)
RETURNS VOID AS $$
BEGIN
    INSERT INTO type_of_ownership(type_of_ownership) VALUES (type_name);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION insert_data_time_type
(
    time_type TEXT,
    number_of_minutes BIGINT,
    cost_per_minute BIGINT
)
RETURNS VOID AS $$
BEGIN
    INSERT INTO time_type(time_type, number_of_minutes, cost_per_minute) VALUES (time_type, number_of_minutes, cost_per_minute);
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION insert_data_customer
(
    name_cust TEXT,
    city_cust TEXT,
    phone_cust BIGINT,
    adres_cust TEXT,
)
RETURNS VOID AS $$
DECLARE
    id_city_cust BIGINT;
BEGIN
    -- Получение id_city_cust
    SELECT a.id_city INTO id_city_cust
    FROM city a
    WHERE a.city = city_cust;

    IF (id_city_cust IS NULL)
    THEN
        RAISE EXCEPTION 'Такого города не существует!';
    END IF;

    -- Вставка данных в таблицу customer
    INSERT INTO customer(name_customer, fid_city, phone_number, address) 
    VALUES (name_cust, id_city_cust, phone_cust::phone_domain, adres_cust);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION insert_data_channel
(
    name_ch TEXT,
    city_ch TEXT,
    type_ownership_ch TEXT,
    employee_ch TEXT, -- Переменная для хранения логина сотрудника
    start_work_ch character varying(5),
    end_work_ch character varying(5)
)
RETURNS VOID AS $$
DECLARE
    city_id BIGINT;
    type_ownership_id BIGINT;
    employee_id TEXT; -- Переменная для хранения id сотрудника
BEGIN

    IF EXISTS (SELECT 1 FROM private_get_emp_channel_data() LIMIT 1)
    THEN
        RAISE EXCEPTION 'У вас уже есть канал!';
    END IF;

    -- Получение id_city
    SELECT a.id_city INTO city_id
    FROM city a
    WHERE a.city = city_ch;

    IF (city_id IS NULL)
    THEN
        RAISE EXCEPTION 'Такого города не существует!';
    END IF;

    -- Получение id_type_of_ownership
    SELECT a.id_type_of_ownership INTO type_ownership_id
    FROM type_of_ownership a
    WHERE a.type_of_ownership = type_ownership_ch;

    IF (type_ownership_id IS NULL)
    THEN
        RAISE EXCEPTION 'Такого типа собственности не существует!';
    END IF;

    -- Получение id сотрудника
    SELECT a.login INTO employee_id
    FROM employee a
    WHERE a.login = employee_ch;



    -- Вставка данных в таблицу channel
    INSERT INTO channel(channel_name, fid_city, fid_type_of_ownership, fid_employee, start_work, end_work) 
    VALUES (name_ch, city_id, type_ownership_id, employee_id, start_work_ch, end_work_ch);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION insert_data_possible_orders
(
    number_p BIGINT,
    channel_p TEXT,
    time_type_p TEXT

)
RETURNS VOID AS $$
DECLARE
    id_channel_p BIGINT;
     id_time_type_p BIGINT;
BEGIN
    -- Получение id_channel_p
    SELECT a.id_channel INTO id_channel_p
    FROM channel a
    WHERE a.channel_name = channel_p;

    IF (id_channel_p IS NULL)
    THEN
        RAISE EXCEPTION 'Такого канала не существует!';
    END IF;

    -- Получение  id_time_type_p
    SELECT a.id_time_type INTO id_time_type_p
    FROM time_type a
    WHERE a.time_type = time_type_p;

    IF (id_time_type_p IS NULL)
    THEN
        RAISE EXCEPTION 'Такого типа времени не существует!';
    END IF;

    -- Вставка данных в таблицу possible_orders
    INSERT INTO possible_orders(number_possible_orders, fid_cannel, fid_time_type) 
    VALUES (number_p, id_channel_p, id_time_type_p);
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION insert_data_orders
(
    customer_o TEXT,
    channel_o TEXT,
    order_date_o DATE,
    time_type_o TEXT,
    duration_o BIGINT,
    possible_order_o BIGINT,
    number_order BIGINT

)
RETURNS VOID AS $$
DECLARE
     owning_ch_rec RECORD; 
     id_customer_o BIGINT;
     id_channel_o BIGINT;
     id_time_type_o BIGINT;
     id_possible_o BIGINT;
     fid_channel_o BIGINT;
BEGIN

     -- Получение owning_ch_rec
    SELECT * INTO owning_ch_rec FROM private_get_emp_channel_data();

    IF (owning_ch_rec IS NULL)
    THEN
        RAISE EXCEPTION 'У вас нет собственного канала!';
    END IF;


       -- Получение id_customer_o
    SELECT a.id_customer INTO id_customer_o
    FROM customer a
    WHERE a.name_customer = customer_o;

    IF (id_customer_o IS NULL)
    THEN
        RAISE EXCEPTION 'Такого заказчика не существует!';
    END IF;

    -- Получение id_channel_o
    SELECT a.id_channel INTO id_channel_o
    FROM channel a
    WHERE a.channel_name = channel_o;

    IF (id_channel_o IS NULL)
    THEN
        RAISE EXCEPTION 'Такого канала не существует!';
    END IF;

    -- Получение  id_time_type_p
    SELECT a.id_time_type INTO id_time_type_o
    FROM time_type a
    WHERE a.time_type = time_type_o;

    IF (id_time_type_o IS NULL)
    THEN
        RAISE EXCEPTION 'Такого типа времени не существует!';
    END IF;

     -- Получение fid_channel_o
    SELECT a.id_possible_orders INTO id_possible_o
    FROM possible_orders a
    WHERE a.number_possible_orders = number_order;

    IF (id_possible_o IS NULL)
    THEN
        RAISE EXCEPTION 'Такого номера заказа не существует!';
    END IF;

         -- Получение fid_channel_o
    SELECT a.id_possible_orders INTO id_possible_o
    FROM possible_orders a
    WHERE a.fid_channel = fid_channel_o;

    IF (id_channel_o != fid_channel_o)
    THEN
        RAISE EXCEPTION 'У возможного заказа другой канал!';
    END IF;

    -- Вставка данных в таблицу orders
    INSERT INTO possible_orders(fid_customer, fid_channel, order_date, fid_time_type, duration, fid_possible_order, number_order) 
    VALUES ( id_customer_o,  id_channel_o, order_date_o,  id_time_type_o, duration_o, id_possible_o, number_order );
END;
$$ LANGUAGE plpgsql;


--UPDATE data functions--
------------


CREATE OR REPLACE FUNCTION update_data_city
(
    id_replace bigint,
    city_name text
)
RETURNS VOID AS $$
BEGIN
    UPDATE city SET city = city_name WHERE id_city = id_replace;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Запись не найдена для обновления!';
    END IF;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_data_role
(
    id_replace bigint,
    role_name text
)
RETURNS VOID AS $$
BEGIN
    UPDATE role SET role = role_name WHERE id_role = id_replace;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Запись не найдена для обновления!';
    END IF;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_data_type_of_ownership
(
    id_replace bigint,
    type_name text
)
RETURNS VOID AS $$
BEGIN
    UPDATE type_of_ownership 
    SET type_of_ownership = type_name 
    WHERE id_type_of_ownership = id_replace;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Запись не найдена для обновления!';
    END IF;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_data_time_type
(
    id_replace bigint,
    time_type_name text,
    number_min bigint,
    cost_min bigint
)
RETURNS VOID AS $$

BEGIN
    UPDATE time_type
    SET    time_type = time_type_name ,
           number_of_minutes = number_min ,
           cost_per_minute = cost_min
    WHERE id_time_type = id_replace;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Запись не найдена для обновления!';
    END IF;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_data_customer
(
    id_replace bigint,
    name_customer text,
    city_name text,
    phone_cust bigint,
    adress_cust text
)
RETURNS VOID AS $$
DECLARE
    id_city_cust BIGINT;
BEGIN

    -- Получение id_city_cust
    SELECT id_city INTO id_city_cust
    FROM city
    WHERE city = city_name;
 

    IF (id_city_cust IS NULL)
    THEN
        RAISE EXCEPTION 'Такого города не существует!';
    END IF;

    -- Вставка данных в таблицу customer
    UPDATE customer
    SET 
        name_customer = name_customer, 
        fid_city = id_city_cust, 
       -- phone_number = phone_cust::phone_domain, 
        phone_number = phone_cust, 
        addres = adress_cust 
    WHERE id_customer = id_replace;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Запись не найдена для обновления!';
    END IF;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION update_data_channel
(
    id_replace bigint,
    name_ch TEXT,
    city TEXT,
    type_ownership TEXT,
    employee_ch TEXT,
    start_time character varying(5),
    end_time character varying(5)
)
RETURNS VOID AS $$
DECLARE
    id_user_ch BIGINT;
    city_id BIGINT;
    type_ownership_id BIGINT;
    employee_id TEXT;
BEGIN

    -- Помещаем результат запроса в переменную
    SELECT id INTO id_user_ch FROM private_get_emp_channel_data() LIMIT 1;

    -- Проверяем, получили ли мы значение в переменной
    IF id_user_ch IS NULL THEN
        RAISE EXCEPTION 'У вас нет канала!';
    END IF;

       -- Получение id_city
    SELECT a.id_sity INTO city_id
    FROM city a
    WHERE a.city = city_ch;

    IF (city_id IS NULL)
    THEN
        RAISE EXCEPTION 'Такого города не существует!';
    END IF;

    -- Получение id_type_of_ownership
    SELECT a.id_type_ownership INTO type_ownership_id
    FROM type_of_ownership a
    WHERE a.type_of_ownership = type_ownership;

    IF (type_ownership_id IS NULL)
    THEN
        RAISE EXCEPTION 'Такого типа собственности не существует!';
    END IF;

    -- Получение id сотрудника
    SELECT a.login INTO employee_id
    FROM employee a
    WHERE a.login = employee_ch;


    -- Вставка данных в таблицу channel
    UPDATE channel
    SET 
        name = name_atc, 
        fid_city = urban_area_id, 
        fid_type_of_ownership = type_ownership_id, 
        login = employee_id, 
        start_work = start_time, 
        end_work = end_time
    WHERE id = id_user_atc AND id_channel = id_replace;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Запись не найдена для обновления!';
    END IF;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION update_data_orders
(
    id_replace bigint,
    customer_o text,
    channel_o text,
    order_date_o date,
    time_type_o text,
    duration_o bigint,
    possible_order_o bigint,
    number_order_o bigint
)
RETURNS VOID AS $$
DECLARE
    owning_ch_rec RECORD;
    id_customer_o BIGINT;
    id_channel_o BIGINT;
    id_time_type_o BIGINT;
    id_possible_o BIGINT;
    fid_channel_o BIGINT;
BEGIN

     -- Получение owning_ch_rec
    SELECT * INTO owning_ch_rec FROM private_get_emp_channel_data();

    IF (owning_ch_rec IS NULL)
    THEN
        RAISE EXCEPTION 'У вас нет собственного канала!';
    END IF;

       -- Получение id_customer_o
    SELECT a.id_customer INTO id_customer_o
    FROM customer a
    WHERE a.name_customer = customer_o;

    IF (id_customer_o IS NULL)
    THEN
        RAISE EXCEPTION 'Такого заказчика не существует!';
    END IF;

    -- Получение id_channel_o
    SELECT a.id_channel INTO id_channel_o
    FROM channel a
    WHERE a.channel_name = channel_o;

    IF (id_channel_o IS NULL)
    THEN
        RAISE EXCEPTION 'Такого канала не существует!';
    END IF;

    -- Получение  id_time_type_o
    SELECT a.id_time_type INTO id_time_type_o
    FROM time_type a
    WHERE a.time_type = time_type_o;

    IF (id_time_type_o IS NULL)
    THEN
        RAISE EXCEPTION 'Такого типа времени не существует!';
    END IF;

     -- Получение id_possible_o
    SELECT a.id_possible_orders INTO id_possible_o
    FROM possible_orders a
    WHERE a.number_possible_orders = number_order;

    IF (id_possible_o IS NULL)
    THEN
        RAISE EXCEPTION 'Такого номера заказа не существует!';
    END IF;

         -- Получение fid_channel_o
    SELECT a.id_possible_orders INTO id_possible_o
    FROM possible_orders a
    WHERE a.fid_channel = fid_channel_o;

    IF (id_channel_o != fid_channel_o)
    THEN
        RAISE EXCEPTION 'У возможного заказа другой канал!';
    END IF;

    -- Вставка данных в таблицу orders
    UPDATE orders
    SET 
        fid_customer =id_customer_o,
        fid_channel =id_channel_o,
        order_date =order_date_o,
        fid_time_type=id_time_type_o,
        duration =duration_o,
        fid_possible_order =id_possible_o,
        number_order =number_order_o

    WHERE id_order = id_replace;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Запись не найдена для обновления!';
    END IF;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION update_data_possible_orders
(
    id_replace bigint,
    number_p bigint,
    channel_p text,
    time_type_p text

)
RETURNS VOID AS $$
DECLARE
    id_channel_p bigint;
     id_time_type_p bigint;
BEGIN
    -- Получение id_channel_p
    SELECT a.id_channel INTO id_channel_p
    FROM channel a
    WHERE a.channel_name = channel_p;

    IF (id_channel_p IS NULL)
    THEN
        RAISE EXCEPTION 'Такого канала не существует!';
    END IF;

    -- Получение  id_time_type_p
    SELECT a.id_time_type INTO id_time_type_p
    FROM time_type a
    WHERE a.time_type = time_type_p;

    IF (id_time_type_p IS NULL)
    THEN
        RAISE EXCEPTION 'Такого типа времени не существует!';
    END IF;

    -- Вставка данных в таблицу possible_orders
    UPDATE possible_orders
    SET 
        number_possible_orders =number_p,
        fid_channel =id_channel_p,
        fid_time_type=id_time_type_p

    WHERE id_possible_orders = id_replace;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Запись не найдена для обновления!';
    END IF;
END;
$$ LANGUAGE plpgsql;

--
--DELETE data functions--
--


CREATE OR REPLACE FUNCTION delete_data_city
(
    id_delete bigint
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM city WHERE id_city = id_delete; 
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Запись не найдена для удаления!';
    END IF;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_data_role
(
    id_delete bigint
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM role WHERE id_role = id_delete; 
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Запись не найдена для удаления!';
    END IF;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_data_type_of_ownership
(
    id_delete bigint
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM type_of_ownership WHERE id_type_of_ownership = id_delete; 
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Запись не найдена для удаления!';
    END IF;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_data_customer
(
    id_delete bigint
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM type_of_ownership WHERE id_customer = id_delete; 
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Запись не найдена для удаления!';
    END IF;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_data_channel()
RETURNS VOID AS $$
BEGIN
    DELETE FROM channel WHERE user_owner = CURRENT_USER; 
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'У вас нет канала!';
    END IF;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_data_channel
(
    id_delete bigint
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM channel WHERE id_channel = id_delete;
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Канал с таким индексом не существует!';
    END IF;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_data_time_type
(
    id_delete bigint
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM time_type WHERE id_time_type = id_delete; 
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Запись не найдена для удаления!';
    END IF;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION delete_data_possible_orders
(
    id_delete bigint
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM possible_orders WHERE id_possible_orders = id_delete; 
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Запись не найдена для удаления!';
    END IF;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION delete_data_orders
(
    id_delete bigint
)
RETURNS VOID AS $$
BEGIN
    DELETE FROM orders WHERE id_order = id_delete; 
    
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Заказ не найден для удаления!';
    END IF;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION count_cascading_deletions_city(id_to_delete bigint)
RETURNS TABLE(table_name TEXT, count_deleted BIGINT) AS $$
DECLARE
    city_count BIGINT;
    customer_count BIGINT;
    channel_count BIGINT;
    orders_count BIGINT;
    possible_orders_count BIGINT;
    total_count BIGINT;
BEGIN
    SELECT COUNT(*) INTO city_count FROM city WHERE id_city = id_to_delete;
    SELECT COUNT(*) INTO customer_count FROM customer WHERE fid_city = id_to_delete;
    SELECT COUNT(*) INTO channel_count FROM channel WHERE fid_city = id_to_delete;
    SELECT COUNT(*) INTO orders_count FROM orders WHERE fid_channel IN (SELECT id_channel FROM channel WHERE fid_city = id_to_delete);
    SELECT COUNT(*) INTO possible_orders_count FROM possible_orders WHERE fid_channel IN (SELECT id_channel FROM channel WHERE fid_city = id_to_delete);
    
    total_count :=city_count + customer_count + channel_count +orders_count + possible_orders_count;

    RETURN QUERY
    SELECT 'Город', city_count
    UNION ALL
    SELECT 'Заказчик', customer_count
    UNION ALL
    SELECT 'Канал', channel_count
    UNION ALL
    SELECT 'Заказ', orders_count
    UNION ALL
    SELECT 'Возможный заказ', possible_orders_count
    UNION ALL
    SELECT 'Всего', total_count;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION count_cascading_deletions_type_of_ownership(id_to_delete bigint)
RETURNS TABLE(table_name TEXT, count_deleted INTEGER) AS $$
DECLARE
    type_of_ownership_count INTEGER;
    channel_count INTEGER;
    orders_count INTEGER;
    possible_orders_count INTEGER;
    total_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO type_of_ownership_count FROM type_of_ownership WHERE id_type_of_ownership = id_to_delete;
    SELECT COUNT(*) INTO channel_count FROM channel WHERE fid_type_of_ownership = id_to_delete;
    SELECT COUNT(*) INTO orders_count FROM orders WHERE fid_channel IN (SELECT id_channel FROM channel WHERE fid_type_of_ownership = id_to_delete);
    SELECT COUNT(*) INTO possible_orders_count FROM possible_orders WHERE fid_channel IN (SELECT id_channel FROM channel WHERE fid_type_of_ownership = id_to_delete);
    
    total_count :=type_of_ownership_count + channel_count + orders_count + possible_orders_count;

    RETURN QUERY
    SELECT 'Тип собственности', type_of_ownership_count
    UNION ALL
    SELECT 'Канал', channel_count
    UNION ALL
    SELECT 'Заказ', orders_count
    UNION ALL
    SELECT 'Возможный заказ', possible_orders_count
    UNION ALL 
    SELECT 'Всего', total_count;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION count_cascading_deletions_time_type(id_to_delete bigint)
RETURNS TABLE(table_name TEXT, count_deleted INTEGER) AS $$
DECLARE
    time_type_count INTEGER;
    orders_count INTEGER;
    possible_orders_count INTEGER;
    total_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO time_type_count FROM time_type WHERE id_time_type = id_to_delete;
    SELECT COUNT(*) INTO orders_count FROM orders WHERE fid_time_type = id_to_delete;
    SELECT COUNT(*) INTO possible_orders_count FROM possible_orders WHERE fid_time_type = id_to_delete;
    
    total_count :=time_type_count + orders_count + possible_orders_count;

    RETURN QUERY
    SELECT 'Тип времени', time_type_count
    UNION ALL
    SELECT 'Заказ', orders_count
    UNION ALL
    SELECT 'Возможный заказ', possible_orders_count
    UNION ALL 
    SELECT 'Всего', total_count;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION count_cascading_deletions_channel(id_to_delete bigint)
RETURNS TABLE(table_name TEXT, count_deleted INTEGER) AS $$
DECLARE
    channel_count INTEGER;
    orders_count INTEGER;
    possible_orders_count INTEGER;
    total_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO channel_count FROM channel WHERE id_channel = id_to_delete;
    SELECT COUNT(*) INTO orders_count FROM orders WHERE fid_channel = id_to_delete;
    SELECT COUNT(*) INTO possible_orders_count FROM possible_orders WHERE fid_channel = id_to_delete;
    
    total_count := channel_count + orders_count + possible_orders_count;

    RETURN QUERY
    SELECT 'Канал', channel_count
    UNION ALL
    SELECT 'Заказ', orders_count
    UNION ALL
    SELECT 'Возможный заказ', possible_orders_count
    UNION ALL 
    SELECT 'Всего', total_count;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION count_cascading_deletions_employee(login_to_delete TEXT)
RETURNS TABLE(table_name TEXT, count_deleted BIGINT) AS $$
DECLARE
    employee_count BIGINT;
    channel_count BIGINT;
    orders_count BIGINT;
    possible_orders_count BIGINT;
    total_count BIGINT;
BEGIN
    SELECT COUNT(*) INTO employee_count FROM employee WHERE login = login_to_delete;
    SELECT COUNT(*) INTO channel_count FROM channel WHERE fid_employee = login_to_delete;
    SELECT COUNT(*) INTO orders_count FROM orders WHERE fid_channel IN (SELECT id_channel FROM channel WHERE fid_employee = login_to_delete);
    SELECT COUNT(*) INTO possible_orders_count FROM possible_orders WHERE fid_channel IN (SELECT id_channel FROM channel WHERE fid_employee = login_to_delete);

    total_count := employee_count + atc_count + drivers_count + cars_count + transportation_count;

    RETURN QUERY
    SELECT 'Сотрудник', employee_count
    UNION ALL
    SELECT 'Канал', channel_count
    UNION ALL
    SELECT 'Заказ', orders_count
    UNION ALL
    SELECT 'Возможный заказ', possible_orders_count
    UNION ALL
    SELECT 'Всего', total_count;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION count_cascading_deletions_customer(id_to_delete bigint)
RETURNS TABLE(table_name TEXT, count_deleted BIGINT) AS $$
DECLARE
    city_count BIGINT;
    customer_count BIGINT;
    channel_count BIGINT;
    orders_count BIGINT;
    possible_orders_count BIGINT;
    total_count BIGINT;
BEGIN
    SELECT COUNT(*) INTO city_count FROM city WHERE id_city = id_to_delete;
    SELECT COUNT(*) INTO customer_count FROM customer WHERE fid_city = id_to_delete;
    SELECT COUNT(*) INTO channel_count FROM channel WHERE fid_city = id_to_delete;
    SELECT COUNT(*) INTO orders_count FROM orders WHERE fid_channel IN (SELECT id_channel FROM channel WHERE fid_city = id_to_delete);
    SELECT COUNT(*) INTO possible_orders_count FROM possible_orders WHERE fid_channel IN (SELECT id_channel FROM channel WHERE fid_city = id_to_delete);
    
    total_count :=city_count + customer_count + channel_count +orders_count + possible_orders_count;

    RETURN QUERY
    SELECT 'Город', city_count
    UNION ALL
    SELECT 'Заказчик', customer_count
    UNION ALL
    SELECT 'Канал', channel_count
    UNION ALL
    SELECT 'Заказ', orders_count
    UNION ALL
    SELECT 'Возможный заказ', possible_orders_count
    UNION ALL
    SELECT 'Всего', total_count;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION count_cascading_deletions_possible_orders(id_to_delete bigint)
RETURNS TABLE(table_name TEXT, count_deleted BIGINT) AS $$
DECLARE 
    possible_orders_count BIGINT;
    orders_count BIGINT;
    total_count BIGINT;
BEGIN
    
    SELECT COUNT(*) INTO possible_orders_count FROM possible_orders WHERE id_possible_orders = id_to_delete;
    SELECT COUNT(*) INTO orders_count FROM orders WHERE  fid_possible_order = id_to_delete;

    total_count :=city_count + customer_count + channel_count +orders_count + possible_orders_count;

    RETURN QUERY
    SELECT 'Возможный заказ', possible_orders_count
    UNION ALL
    SELECT 'Заказ', orders_count
    UNION ALL
    SELECT 'Всего', total_count;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION count_cascading_deletions_orders(id_to_delete bigint)
RETURNS TABLE(table_name TEXT, count_deleted BIGINT) AS $$
DECLARE
    orders_count BIGINT;
    possible_orders_count BIGINT;
    total_count BIGINT;
BEGIN
    SELECT COUNT(*) INTO orders_count FROM orders WHERE id_order = id_to_delete;
    SELECT COUNT(*) INTO possible_orders_count FROM orders WHERE fid_possible_order = id_to_delete;
    
    total_count := orders_count + possible_orders_count;

    RETURN QUERY
    SELECT 'Заказ', orders_count
    UNION ALL
    SELECT 'Возможный заказ', possible_orders_count
    UNION ALL
    SELECT 'Всего', total_count;
END;
$$ LANGUAGE plpgsql;