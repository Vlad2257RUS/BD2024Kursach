--Удаляем всех пользователей
DO $$
DECLARE
    username TEXT;
BEGIN
    FOR username IN SELECT usename FROM pg_catalog.pg_user WHERE usename <> 'postgres' LOOP
        EXECUTE format('DROP ROLE IF EXISTS %I;', username);
    END LOOP;
END $$;

--Удаление зависимых объектов
DROP FUNCTION IF EXISTS create_new_user_by_role;

--Удаление ролей
DO $$
DECLARE
    role_name TEXT;
    role_cursor CURSOR FOR SELECT rolname FROM pg_roles WHERE rolname IN ('emp_ch', 'administrator', 'analyst');
BEGIN
    FOR role_rec IN role_cursor LOOP
        role_name := role_rec.rolname;
        IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = role_name) THEN
            EXECUTE format('DROP ROLE IF EXISTS %I', role_name);
        END IF;
    END LOOP;
END $$;

--Создание и настройка ролей
CREATE ROLE emp_ch;

GRANT USAGE ON SCHEMA public TO emp_ch;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO emp_ch;
GRANT TRIGGER ON ALL TABLES IN SCHEMA public TO emp_ch;

GRANT SELECT ON ALL TABLES IN SCHEMA public TO emp_ch;
REVOKE SELECT, INSERT, UPDATE, DELETE ON public.employee FROM emp_ch;
REVOKE INSERT, UPDATE, DELETE ON public.channel FROM emp_ch;

GRANT ALL ON TABLE public.orders TO emp_ch;
GRANT ALL ON TABLE public.possible_orders TO emp_ch;

CREATE ROLE administrator;

GRANT USAGE ON SCHEMA public TO administrator;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO administrator;
GRANT TRIGGER ON ALL TABLES IN SCHEMA public TO administrator;

REVOKE ALL ON public.channel FROM administrator;
REVOKE ALL ON public.orders FROM administrator;
REVOKE ALL ON public.possible_orders FROM administrator;

GRANT ALL ON TABLE public.city TO administrator;
GRANT ALL ON TABLE public.customer TO administrator;
GRANT ALL ON TABLE public.type_of_ownership TO administrator;
GRANT ALL ON TABLE public.employee TO administrator;
GRANT ALL ON TABLE public.role TO administrator;
GRANT ALL ON TABLE public.time_type TO administrator;

GRANT SELECT (id_channel, fid_city, fid_type_of_ownership) ON TABLE public.channel TO administrator;
GRANT SELECT (id_order, fid_channel, fid_time_type) ON TABLE public.orders TO administrator;
GRANT SELECT (id_possible_orders, fid_time_type) ON TABLE public.possible_orders TO administrator;

CREATE ROLE analyst;
GRANT USAGE ON SCHEMA public TO analyst;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO analyst;
REVOKE ALL ON TABLE public.employee FROM analyst;