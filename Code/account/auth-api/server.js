// imports
import express from 'express';
import cors from 'cors';
import dotenv from 'dotenv';
import * as bcrypt from 'bcryptjs';
import { pool } from './db.js';
import artifactRoutes from './artifactRoutes.js';

import aiRoutes from './aiRoutes.js';
import vectorSearchRouter from './vectorSearchRouter.js';

// server set up
dotenv.config()
const app = express()
app.use(cors());
app.use(express.json());
app.use('/api/artifacts', artifactRoutes);
app.use('/api/ai', aiRoutes);
app.use('/search', vectorSearchRouter);  // raw vector search (returns chunks)

const PORT = process.env.PORT || 4000;

// signup method
app.post('/api/auth/signup', async (req, res) => {

    // get user information from form
    try {
        const { email, username, password } = req.body;

        // error check for missing information
        if (!email || !username || !password) {
            return res.status(400).json({
                ok: false,
                error: 'Missing field(s).'
            });
        }

        // trim inputs
        const trimmedEmail = email.trim();
        const trimmedUsername = username.trim();

        // check if email or username already exists
        const existingUserQuery = `
            SELECT user_id
            FROM users
            WHERE email = $1 OR username = $2
            LIMIT 1
        `;

        // send query
        const existingUserResult = await pool.query(existingUserQuery, [
            trimmedEmail,
            trimmedUsername
        ]);

        // return message if names already taken
        if (existingUserResult.rows.length > 0) {
            return res.status(409).json({
                ok: false,
                error: 'Email/Username already taken'
            });
        }

        // hash password
        const hashedPassword = await bcrypt.hash(password, 12);

        // sql query
        const query = `INSERT INTO users
                       (email, username, password_hash)
                       VALUES ($1, $2, $3)
                       RETURNING user_id, email, username, created_at`;

        // send query to Postgres
        const { rows } = await pool.query(query, [trimmedEmail, trimmedUsername, hashedPassword]);
        res.json({ ok: true, user: rows[0] });
    }

    // error handling
    catch (error) {
        if (/(unique)/i.test(String(error.message))) {
            return res.status(409).json({
                ok: false,
                error: 'Email or username already exists.'
            })
        }

        console.error(error);
        res.status(500).json({ ok: false, error: 'Server error.' });
    }
});

// login method
app.post('/api/auth/login', async (req, res) => {
    console.log('Received login request:', req.body);
    // get user information from form
    try {
        const { username, password } = req.body;

        if (!username || !password) {
            return res.status(400).json({
                ok: false,
                error: 'Missing field(s).'
            });
        }

        // trim input
        const trimmedUsername = username.trim();

        // sql query
        const query = `SELECT user_id, password_hash
                       FROM users
                       WHERE username = $1`;

        // send query to Postgres
        const { rows } = await pool.query(query, [trimmedUsername]);

        // error check user information
        if (rows.length === 0) {
            return res.status(401).json({ ok: false, error: 'Incorrect username/password.' });
        }

        // compare password and hashed password
        const match = await bcrypt.compare(password, rows[0].password_hash);
        if (!match) {
            return res.status(401).json({ ok: false, error: 'Incorrect username/password.' });
        }

        res.json({ ok: true, userId: rows[0].user_id });
    }

    // error handling
    catch (error) {
        console.error(error);
        res.status(500).json({ ok: false, error: 'Server error.' });
    }
});

app.listen(PORT, () =>
    console.log(`Auth API running on http://localhost:${PORT}`)
);