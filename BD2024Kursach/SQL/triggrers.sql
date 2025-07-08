CREATE TRIGGER delete_city
AFTER DELETE ON city
FOR EACH ROW
AS
BEGIN
  -- Удаляем все связанные записи из других таблиц после удаления города
 DELETE FROM channel WHERE fid_city = OLD.id_city;
DELETE FROM customer WHERE fid_city = OLD.id_city; 
END;




CREATE OR REPLACE TRIGGER trigger_cost
    BEFORE INSERT OR UPDATE 
    ON public.time_type
    FOR EACH ROW
    EXECUTE FUNCTION public.trigger_cost();

--For table time_type
CREATE OR REPLACE FUNCTION public.trigger_cost()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    VOLATILE
    COST 100
AS $$
BEGIN
IF (NEW.cost_per_minute < 10000) THEN
        RAISE EXCEPTION 'Стоимость рекламы не может быть ниже 10000 рублей за минуту!';
    END IF;
	RETURN NEW;
END; $$;



CREATE OR REPLACE TRIGGER triggerphone_number
    BEFORE INSERT OR UPDATE OF phone_number
    ON public.customer
    FOR EACH ROW
    EXECUTE FUNCTION public.trigger_customer_phone_number();

--For table customer
CREATE OR REPLACE FUNCTION public.trigger_customer_phone_number()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    VOLATILE
    COST 100
AS $BODY$
BEGIN
IF (NEW.phone_number < 1) THEN
RAISE EXCEPTION 'Телефон не может иметь отрицательное значение';
END IF;
RETURN NEW;
END;


CREATE OR REPLACE TRIGGER trigger_del_channel
    BEFORE DELETE
    ON public." channel"
    FOR EACH ROW
    EXECUTE FUNCTION public.trigger_del_channel();

--For table channel
CREATE OR REPLACE FUNCTION public.trigger_del_channel()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    VOLATILE
    COST 100
AS $BODY$
BEGIN
  -- Удаление записей из таблицы Роли
  DELETE FROM employee WHERE fid_channel =OLD.id_channel;

  -- Удаление записей из таблицы Заказы
  DELETE FROM orders WHERE fid_channel =OLD.id_channel;

  DELETE FROM possible_orders WHERE fid_channel =OLD.id_channel;

END;



CREATE OR REPLACE TRIGGER trigger_employeet
    BEFORE INSERT OR UPDATE 
    ON public.employee
    FOR EACH ROW
    EXECUTE FUNCTION public.trigger_employee();

--For table employee
CREATE OR REPLACE FUNCTION public.trigger_employee()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    VOLATILE
    COST 100
AS $BODY$
BEGIN
IF(NEW.fid_channel = OLD.fid_channel) THEN
        RAISE EXCEPTION 'У канала уже есть сотрудник';
    END IF;
	RETURN NEW;
END;



CREATE OR REPLACE TRIGGER trigger_order
    BEFORE INSERT OR UPDATE 
    ON public.orders
    FOR EACH ROW
    EXECUTE FUNCTION public.trigger_order();

--For table order
CREATE OR REPLACE FUNCTION public.trigger_order()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    VOLATILE
    COST 100
AS $BODY$
BEGIN
IF(New.order_date<'1991-01-01'::DATE) THEN
RAISE EXCEPTION 'Год заказа не может быть меньше 1991 г.!';
END IF;
RETURN NEW;
END;

CREATE OR REPLACE TRIGGER trigger_time_type
    BEFORE INSERT OR UPDATE 
    ON public.time_type
    FOR EACH ROW
    EXECUTE FUNCTION public.trigger_time_type();

--For table time_type
CREATE OR REPLACE FUNCTION public.trigger_time_type()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    VOLATILE
    COST 100
AS $BODY$
BEGIN

    IF (NEW.number_of_minutes > 12) THEN
        RAISE EXCEPTION 'Реклама не может быть больше 12 минут!';
		ELSIF(NEW.number_of_minutes < 1) THEN
        RAISE EXCEPTION 'Время рекламы не может быть отрицательным!';
    END IF;
	RETURN NEW;
END;



CREATE OR REPLACE TRIGGER trigger_truncate_type_of_ownership
    BEFORE TRUNCATE
    ON public.type_of_ownership
    FOR EACH STATEMENT
    EXECUTE FUNCTION public.trigger_truncate_type_of_ownership();

--For table type_of_ownership
CREATE OR REPLACE FUNCTION public.trigger_truncate_type_of_ownership()
    RETURNS trigger
    LANGUAGE 'plpgsql'

AS $$
BEGIN
IF (NEW.type_of_ownership='Очистить') THEN
  INSERT INTO type_of_ownership (
    id_type_of_ownership,
    type_of_ownership
    
  ) VALUES (
    0,
    'Государственный'
  );
  UPDATE channel
SET fid_type_of_ownership = 0;
  END IF;
   RETURN NEW;
END  $$   LANGUAGE 'plpgsql';


CREATE OR REPLACE TRIGGER trigger_timework
    BEFORE INSERT OR UPDATE 
    ON public." channel"
    FOR EACH ROW
    EXECUTE FUNCTION public.trigger_update_or_insert_timework();


--ForTable orders
CREATE OR REPLACE FUNCTION public.trigger_update_or_insert_timework()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    VOLATILE
    COST 100
AS $$
BEGIN
IF (NEW.start_work >=NEW.end_work) THEN
 RAISE EXCEPTION 'Время начала работы канала должно быть меньше времени окончания работы';
END IF;

RETURN NEW;
END; $$;

