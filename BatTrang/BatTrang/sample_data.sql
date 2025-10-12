-- Sample data for BatTrang database

-- Insert Categories
INSERT INTO dbo.categories (name) VALUES 
('Bát ăn cơm'),
('Đĩa ăn'),
('Ấm trà'),
('Bình hoa'),
('Tượng trang trí');

-- Insert Products
INSERT INTO dbo.products (category_id, name, image_url, description, price) VALUES 
(1, 'Bát ăn cơm men xanh', 'https://via.placeholder.com/300x300', 'Bát ăn cơm truyền thống Bát Tràng với men xanh đặc trưng', 45000),
(1, 'Bát ăn cơm men trắng', 'https://via.placeholder.com/300x300', 'Bát ăn cơm men trắng tinh khiết', 40000),
(2, 'Đĩa ăn men xanh', 'https://via.placeholder.com/300x300', 'Đĩa ăn men xanh Bát Tràng', 65000),
(2, 'Đĩa ăn men trắng', 'https://via.placeholder.com/300x300', 'Đĩa ăn men trắng cao cấp', 60000),
(3, 'Ấm trà men xanh', 'https://via.placeholder.com/300x300', 'Ấm trà truyền thống Bát Tràng', 120000),
(3, 'Ấm trà men trắng', 'https://via.placeholder.com/300x300', 'Ấm trà men trắng tinh xảo', 110000),
(4, 'Bình hoa men xanh', 'https://via.placeholder.com/300x300', 'Bình hoa trang trí men xanh', 85000),
(4, 'Bình hoa men trắng', 'https://via.placeholder.com/300x300', 'Bình hoa trang trí men trắng', 80000),
(5, 'Tượng Phật men xanh', 'https://via.placeholder.com/300x300', 'Tượng Phật trang trí men xanh', 200000),
(5, 'Tượng Phật men trắng', 'https://via.placeholder.com/300x300', 'Tượng Phật trang trí men trắng', 180000);

-- Insert Sample User
INSERT INTO dbo.users (username, password) VALUES 
('admin', '123456'),
('user1', '123456');
