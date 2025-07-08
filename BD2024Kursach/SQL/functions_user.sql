-- Сначала удаляем существующий ENUM
DROP TYPE IF EXISTS roles;

-- Затем создаем новый ENUM
CREATE TYPE roles AS ENUM ('emp_ch', 'administrator', 'analyst');

-- Создаем нового пользователя с определенной ролью
CREATE OR REPLACE FUNCTION create_new_user_by_role
(
    user_login text, 
    pass text, 
    user_role roles,
    employee text,   
    user_data JSONB
)
RETURNS VOID AS $$
BEGIN
    EXECUTE format('CREATE USER %I WITH PASSWORD %L', user_login, pass);

    CASE user_role
        WHEN 'emp_ch' THEN
            EXECUTE format('GRANT emp_ch TO %I', user_login);
        WHEN 'administrator' THEN
            EXECUTE format('GRANT administrator TO %I', user_login);
        WHEN 'analyst' THEN
            EXECUTE format('GRANT analyst TO %I', user_login);
    END CASE;

    INSERT INTO employee (login, employee ) VALUES (user_login, employee);
END;
$$ LANGUAGE plpgsql;

-- Удаляем пользователя по логину
CREATE OR REPLACE FUNCTION delete_user_by_login
(
    user_login text
)
RETURNS VOID AS $$
BEGIN
    EXECUTE format('DROP USER %I', user_login);

    EXECUTE format('DELETE FROM emplotee WHERE login = %L', user_login);
END;
$$ LANGUAGE plpgsql;

-- Получаем названия всех таблиц к которым есть доступ у пользователя

CREATE OR REPLACE FUNCTION get_available_tables_between_role()
RETURNS TABLE ("Таблицы" text) AS $$
BEGIN
    IF ((SELECT pg_has_role(CURRENT_USER, 'emp_ch', 'MEMBER')) OR (SELECT pg_has_role(CURRENT_USER, 'administrator', 'MEMBER')))
    THEN
        RETURN QUERY 
        SELECT table_name::text
        FROM information_schema.tables
        WHERE table_schema = 'public' 
        AND table_type = 'BASE TABLE'
        AND has_table_privilege(
            quote_ident(table_schema) || '.' || quote_ident(table_name),
            'SELECT'
        )
        AND has_table_privilege(
            quote_ident(table_schema) || '.' || quote_ident(table_name),
            'INSERT'
        )
        AND has_table_privilege(
            quote_ident(table_schema) || '.' || quote_ident(table_name),
            'UPDATE'
        )
        AND has_table_privilege(
            quote_ident(table_schema) || '.' || quote_ident(table_name),
            'DELETE'
        );
    ELSIF (SELECT pg_has_role(CURRENT_USER, 'analyst', 'MEMBER'))
    THEN
        RETURN QUERY 
        SELECT table_name::text
        FROM information_schema.tables
        WHERE table_schema = 'public' 
        AND table_type = 'BASE TABLE'
        AND has_table_privilege(
            quote_ident(table_schema) || '.' || quote_ident(table_name),
            'SELECT'
        );
    END IF;
END;
$$ LANGUAGE plpgsql;

-- Получаем роль пользователя 
CREATE OR REPLACE FUNCTION get_role_user()
RETURNS roles AS $$
BEGIN
    IF (SELECT pg_has_role(CURRENT_USER, 'emp_ch', 'MEMBER'))
    THEN
        RETURN 'emp_ch';
    ELSIF (SELECT pg_has_role(CURRENT_USER, 'analyst', 'MEMBER'))
    THEN
        RETURN 'analyst';
    ELSIF (SELECT pg_has_role(CURRENT_USER, 'administrator', 'MEMBER'))
    THEN
        RETURN 'administrator';
	END IF;
END;
$$ LANGUAGE plpgsql;

-- Получить всех пользователей у которых нет канала
CREATE OR REPLACE FUNCTION get_free_ch_owners(generate_limit BIGINT)
RETURNS TABLE (login TEXT)	
AS $$
BEGIN
    RETURN QUERY
    SELECT u.login
    FROM employee u 
    LEFT JOIN channel a ON u.login = a.fid_employee
    WHERE pg_has_role(u.login, 'emp_ch', 'MEMBER') 
    AND a.fid_employee IS NULL
    LIMIT generate_limit;
END;
$$ LANGUAGE plpgsql;
