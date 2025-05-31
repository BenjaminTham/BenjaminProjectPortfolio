DROP TABLE IF EXISTS logs;
DROP TABLE IF EXISTS mantraps;
DROP TABLE IF EXISTS users;

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(255) UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    role VARCHAR(50) NOT NULL,
    biometrics_enrolled BOOLEAN DEFAULT FALSE,
    alert_subscribed BOOLEAN DEFAULT FALSE,
    failed_attempts INT DEFAULT 0,
    location VARCHAR(255) DEFAULT 'Outside',
    token TEXT,
    token_created TIMESTAMP
);

CREATE TABLE mantraps (
    id SERIAL PRIMARY KEY,
    location VARCHAR(255) NOT NULL,
    token TEXT UNIQUE NOT NULL,
    entry_gate_status VARCHAR(50) NOT NULL DEFAULT 'CLOSED',
    exit_gate_status VARCHAR(50) NOT NULL DEFAULT 'CLOSED'
);

CREATE TABLE logs (
    id SERIAL PRIMARY KEY,
    category VARCHAR(100) NOT NULL,
    user_id INT REFERENCES users(id) ON DELETE SET NULL,
    mantrap_id INT REFERENCES mantraps(id) ON DELETE SET NULL,
    message TEXT NOT NULL,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    file TEXT
);

INSERT INTO users (username, email, password_hash, role) VALUES ('admin', 'admin@spmovy.com', '$2a$12$I8uvad785z9c.9lpx5.yKu4qvp6v0qAoPltBd7DmZijjHAAUnzCi6', 'admin');
INSERT INTO users (username, email, password_hash, role, alert_subscribed) VALUES ('limcheehean', 'lychee0504@gmail.com', '$2a$12$I8uvad785z9c.9lpx5.yKu4qvp6v0qAoPltBd7DmZijjHAAUnzCi6', 'admin', true);
INSERT INTO users (username, email, password_hash, role, alert_subscribed) VALUES ('ernestfoo', 'Foo.YongJie.Ernest@gmail.com', '$2a$12$I8uvad785z9c.9lpx5.yKu4qvp6v0qAoPltBd7DmZijjHAAUnzCi6', 'admin', false);
INSERT INTO mantraps (location, token) VALUES ('E2-05-05', 'a1b2c3d4e5d6f7')