
--ROLE LEVEL SECURITY
-- Создаем политики безопасности для таблицы channel
CREATE POLICY restrict_access_to_the_selected_channel ON public.channel
FOR SELECT
TO emp_ch
USING (fid_employee = CURRENT_USER);

CREATE POLICY allow_select_channel_access_for_analyst_or_administrator ON public.channel
FOR SELECT
TO analyst, administrator
USING (true);

CREATE POLICY restrict_insert_channel_access ON public.channel
FOR INSERT
TO emp_ch
WITH CHECK (fid_employee = CURRENT_USER);

CREATE POLICY restrict_update_channel_access ON public.channel
FOR UPDATE
TO emp_ch
USING (fid_employee = CURRENT_USER);

CREATE POLICY restrict_delete_channel_access ON public.channel
FOR DELETE
TO emp_ch
USING (fid_employee = CURRENT_USER);
-- Создаем политики безопасности для таблицы orders

CREATE POLICY restrict_insert_orders_access ON public.orders
FOR INSERT
TO emp_ch
WITH CHECK (
    EXISTS (
        SELECT 1
        FROM channel
        WHERE channel.id_channel = orders.fid_channel
        AND channel.fid_employee = CURRENT_USER 
    )
);

CREATE POLICY restrict_update_orders_access ON public.orders
FOR UPDATE
TO emp_ch
WITH CHECK (
    EXISTS (
        SELECT 1
        FROM channel
        WHERE channel.id_channel = orders.fid_channel
        AND channel.fid_employee = CURRENT_USER 
    )
);

CREATE POLICY restrict_delete_orders_access ON public.orders
FOR DELETE
TO emp_ch
USING (
    EXISTS (
        SELECT 1
        FROM channel
        WHERE channel.id_channel = orders.fid_channel
        AND channel.fid_employee = CURRENT_USER 
    )
);

CREATE POLICY restrict_select_orders_access ON public.orders
FOR SELECT
TO emp_ch
USING (
    EXISTS (
        SELECT 1
        FROM channel
        WHERE channel.id_channel = orders.fid_channel
        AND channel.fid_employee = CURRENT_USER 
    )
);

CREATE POLICY allow_select_orders_for_analyst_or_administrator ON public.orders
FOR SELECT
TO analyst, administrator
USING (true);

-- Создаем политики безопасности для таблицы possible_orders

CREATE POLICY allow_select_possible_orders_access_for_analyst_or_employee ON public.possible_orders
FOR SELECT
TO emp_ch, analyst
USING (true);

CREATE POLICY allow_insert_possible_orders_for_employee ON public.possible_orders
FOR INSERT
TO emp_ch
WITH CHECK (true);

CREATE POLICY allow_update_possible_orders_for_employee ON public.possible_orders
FOR UPDATE
TO emp_ch
USING (true);

CREATE POLICY allow_delete_possible_orders_for_employee ON public.possible_orders
FOR DELETE
TO emp_ch
USING (true);

-- Создаем политики безопасности для таблицы employee

CREATE POLICY allow_select_employee_for_analyst_or_administrator ON public.employee
FOR SELECT
TO administrator, analyst
USING (true);

CREATE POLICY allow_incert_employee_for_administrator ON public.employee
FOR INSERT
TO administrator
WITH CHECK (true);

CREATE POLICY allow_update_employee_for_administrator ON public.employee
FOR UPDATE
TO administrator
USING (true);

CREATE POLICY allow_delete_employee_for_administrator ON public.employee
FOR DELETE
TO administrator
USING (true);

-- Создаем политики безопасности для таблицы customer

CREATE POLICY allow_select_customer_for_analyst_or_administrator ON public.customer
FOR SELECT
TO administrator, analyst
USING (true);

CREATE POLICY allow_insert_customer_for_administrator ON public.customer
FOR INSERT
TO administrator
WITH CHECK (true);

CREATE POLICY allow_update_customer_for_administrator ON public.customer
FOR UPDATE
TO administrator
USING (true);

CREATE POLICY allow_delete_customer_for_administrator ON public.customer
FOR DELETE
TO administrator
USING (true);

-- Создаем политики безопасности для таблицы city

CREATE POLICY allow_select_city_for_analyst_or_administrator ON public.city
FOR SELECT
TO administrator, analyst
USING (true);

CREATE POLICY allow_incert_city_for_administrator ON public.city
FOR INSERT
TO administrator
WITH CHECK (true);

CREATE POLICY allow_update_city_for_administrator ON public.city
FOR UPDATE
TO administrator
USING (true);

CREATE POLICY allow_delete_city_for_administrator ON public.city
FOR DELETE
TO administrator
USING (true);


-- Создаем политики безопасности для таблицы time_type

CREATE POLICY allow_select_time_type_for_analyst_or_administrator ON public.time_type
FOR SELECT
TO administrator, analyst
USING (true);

CREATE POLICY allow_incert_time_type_for_administrator ON public.time_type
FOR INSERT
TO administrator
WITH CHECK (true);

CREATE POLICY allow_update_time_type_for_administrator ON public.time_type
FOR UPDATE
TO administrator
USING (true);

CREATE POLICY allow_delete_time_type_for_administrator ON public.time_type
FOR DELETE
TO administrator
USING (true);


-- Создаем политики безопасности для таблицы type_of_ownership

CREATE POLICY allow_select_type_of_ownership_for_analyst_or_administrator ON public.type_of_ownership
FOR SELECT
TO administrator, analyst
USING (true);

CREATE POLICY allow_incert_type_of_ownership_for_administrator ON public.type_of_ownership
FOR INSERT
TO administrator
WITH CHECK (true);

CREATE POLICY allow_update_type_of_ownership_for_administrator ON public.type_of_ownership
FOR UPDATE
TO administrator
USING (true);

CREATE POLICY allow_delete_type_of_ownership_for_administrator ON public.type_of_ownership
FOR DELETE
TO administrator
USING (true);
