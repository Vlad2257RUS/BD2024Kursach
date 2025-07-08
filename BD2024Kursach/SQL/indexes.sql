-- channel
CREATE INDEX idx_ch_fid_employee ON channel (fid_employee);

--employee
CREATE INDEX idx_users_login ON employee(login);

-- transportation
-- Индексы для обычной таблицы
CREATE INDEX idx_orders_id_cargo ON orders (fid_customer);
CREATE INDEX idx_orders_fid_channel ON orders (fid_channel);
CREATE INDEX idx_orders_order_date ON orders (order_date);
CREATE INDEX idx_orders_fid_time_type ON orders fid_time_type);
CREATE INDEX idx_orders_fid_possible_order ON orders (fid_possible_order);

-- channel
-- Индексы для обычной таблицы

CREATE INDEX idx_channel_employee ON channel (fid_employee);

CREATE INDEX idx_channel_end_work ON channel (end_work);

