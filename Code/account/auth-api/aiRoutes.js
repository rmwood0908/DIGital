import express from 'express';
import dotenv from 'dotenv';
const router = express.Router();
import axios from "axios";

dotenv.config();

router.post('/', async (req, res) => {
    try {

    const { prompt } = req.body;

    if (!prompt) {
      return res.status(400).json({
        ok: false,
        error: 'Prompt is required',
      });
    }

    const response = await axios.post(
      'https://api.mistral.ai/v1/chat/completions',
      {
        model: 'mistral-small',
        messages: [
          {
            role: 'user',
            content: prompt,
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

    console.log('Mistral response:', aiMessage);

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