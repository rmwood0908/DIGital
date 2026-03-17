// main vector search logic for retrieving relevant chunks based on cosine similarity of embeddings

import express from 'express';
import fs from 'fs';
import { embed } from './embeddings.js';

const router = express.Router();

let db = null;

function getDB()
{
    if (!db)
    {
        if (!fs.existsSync('./embeddings.json'))
        {
            throw new Error('embeddings.json not found. Run: node generateEmbeddings.js');
        }
        db = JSON.parse(fs.readFileSync('./embeddings.json', 'utf-8'));
        console.log(`Loaded ${db.chunks.length} chunks from embeddings.json`);
    }
    return db;
}

function cosineSimilarity(a, b)
{
    let dot = 0, normA = 0, normB = 0;
    for (let i = 0; i < a.length; i++)
    {
        dot += a[i] * b[i];
        normA += a[i] * a[i];
        normB += b[i] * b[i];
    }
    return dot / (Math.sqrt(normA) * Math.sqrt(normB));
}

export function search(queryEmbedding, topK = 5)
{
    const db = getDB();
    return db.chunks
        .map(chunk => ({
            score: cosineSimilarity(queryEmbedding, chunk.embedding),
            text: chunk.text,
            filename: chunk.filename,
            chunkIndex: chunk.chunkIndex,
        }))
        .sort((a, b) => b.score - a.score)
        .slice(0, topK);
}

// POST /search
// Body: { query: string, topK?: number }
router.post('/', async (req, res) =>
{
    try
    {
        const { query, topK = 5 } = req.body;

        if (!query)
        {
            return res.status(400).json({ ok: false, error: 'query is required' });
        }

        const queryEmbedding = await embed(query);
        const results = search(queryEmbedding, topK);

        res.status(200).json({ ok: true, results });

    }
    catch (error)
    {
        console.error('Vector search error:', error.message);
        res.status(500).json({ ok: false, error: error.message });
    }
});

export default router;
