import express from 'express';
import dotenv from 'dotenv';
const router = express.Router();
import axios from "axios";

import { embed } from './embeddings.js';
import { search } from './vectorSearchRouter.js';

dotenv.config();

// main ai route called from unity
// expects { prompt: string, topK: number (optional) }
// takes user's prompt, searches for relevant document chunks, and returns an AI-generated answer based on those chunks
router.post('/', async (req, res) => {
    try {

    const { prompt, topK = 5 } = req.body;

    if (!prompt) {
      return res.status(400).json({
        ok: false,
        error: 'Prompt is required',
      });
    }

    // 1. Embed the user query
    const queryEmbedding = await embed(prompt);

    // 2. Search for relevant chunks
    const results = search(queryEmbedding, topK);

    // 3. Build context string
    const context = results
      .map(r => `From ${r.filename} (chunk ${r.chunkIndex}):\n${r.text}`)
      .join("\n\n");

    // 4. Build RAG prompt
    const ragPrompt = `
      You are an assistant that answers questions using the provided context.

      CONTEXT:
      ${context}

      USER QUESTION:
      ${prompt}

      If the answer is not in the context, say "I don't know based on the provided documents."
      `;

    const response = await axios.post(
      'https://api.mistral.ai/v1/chat/completions',
      {
        model: 'mistral-small',
        messages: [
          {
            role: 'user',
            content: ragPrompt,
          },
        ],
        temperature: 0.7,
      },
      {
        headers: {
          Authorization: `Bearer ${process.env.APIKEY}`,
          'Content-Type': 'application/json',
        },
      }
    );

    const aiMessage =
      response.data.choices[0].message.content;

    // console.log('Mistral response:', aiMessage);

    res.status(200).json({
      ok: true,
      reply: aiMessage,
    });

  } catch (error) {
    console.error(
      'Mistral API error:',
      error.response?.data || error.message
    );

    res.status(500).json({
      ok: false,
      error: 'Failed to get AI response',
    });
  }
});

export default router;