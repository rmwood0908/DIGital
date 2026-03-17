// generates embeddings for all .txt files in the Docs folder and saves to embeddings.json 
// Run once (or whenever your .txt files change) using:
//   node generateEmbeddings.js

import fs from 'fs';
import path from 'path';
import { pipeline } from '@xenova/transformers';

const TXT_FOLDER = './Docs';
const OUTPUT_FILE = './embeddings.json';
const MAX_CHUNK_CHARS = 2000; // safe for nomic-embed-text (8192 token limit)

async function embed(embedder, text)
{
    const output = await embedder(text, { pooling: 'mean', normalize: true });
    return Array.from(output.data);
}

function chunkText(text, maxChars)
{
    const sentences = text.split(/(?<=[.!?])\s+/);
    const chunks = [];
    let current = '';

    for (const sentence of sentences)
    {
        if ((current + sentence).length > maxChars && current.length > 0)
        {
            chunks.push(current.trim());
            current = '';
        }
        current += sentence + ' ';
    }
    if (current.trim()) chunks.push(current.trim());
    return chunks;
}

async function main()
{
    if (!fs.existsSync(TXT_FOLDER))
    {
        console.error(`Folder "${TXT_FOLDER}" not found. Create it and add your .txt files.`);
        process.exit(1);
    }

    const files = fs.readdirSync(TXT_FOLDER).filter(f => f.endsWith('.txt'));

    if (files.length === 0)
    {
        console.error(`No .txt files found in "${TXT_FOLDER}".`);
        process.exit(1);
    }

    console.log(`Found ${files.length} file(s). Loading embedding model...`);
    const embedder = await pipeline('feature-extraction', 'Xenova/nomic-embed-text-v1');
    console.log('Model ready. Generating embeddings...');

    const db = { chunks: [] };

    for (const file of files)
    {
        const text = fs.readFileSync(path.join(TXT_FOLDER, file), 'utf-8');
        const chunks = chunkText(text, MAX_CHUNK_CHARS);

        console.log(`${file} -> ${chunks.length} chunk(s)`);

        for (let i = 0; i < chunks.length; i++)
        {
            const embedding = await embed(embedder, chunks[i]);
            db.chunks.push({
                filename: file,
                chunkIndex: i,
                text: chunks[i],
                embedding,
            });
            console.log(`  chunk ${i + 1}/${chunks.length} done`);
        }
    }

    fs.writeFileSync(OUTPUT_FILE, JSON.stringify(db, null, 2));
    console.log(`Done. ${db.chunks.length} total chunks saved to ${OUTPUT_FILE}`);
}

main().catch(err =>
{
    console.error('Error:', err.message);
    process.exit(1);
});
