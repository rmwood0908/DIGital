// gets the llm used to embed text and convert it to vectors for vector search
// needed for the main vector search
import { pipeline } from '@xenova/transformers';

let embedder = null;

export async function getEmbedder()
{
    if (!embedder)
    {
        console.log('Loading nomic-embed-text model...');
        embedder = await pipeline('feature-extraction', 'Xenova/nomic-embed-text-v1');
        console.log('Embedding model ready');
    }
    return embedder;
}

export async function embed(text)
{
    const model = await getEmbedder();
    const output = await model(text, { pooling: 'mean', normalize: true });
    return Array.from(output.data);
}