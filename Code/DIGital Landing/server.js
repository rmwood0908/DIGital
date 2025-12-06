import express from 'express';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const app = express();
const PORT = process.env.PORT || 3000;

app.use((req, res, next) => {

  if (req.url.endsWith('.js.br')) {
    res.set('Content-Encoding', 'br');
    res.set('Content-Type', 'application/javascript');
  } else if (req.url.endsWith('.wasm.br')) {
    res.set('Content-Encoding', 'br');
    res.set('Content-Type', 'application/wasm');
  } else if (req.url.endsWith('.data.br')) {
    res.set('Content-Encoding', 'br');
    res.set('Content-Type', 'application/octet-stream');
  } else if (req.url.endsWith('.symbols.json.br')) {
    res.set('Content-Encoding', 'br');
    res.set('Content-Type', 'application/json');
  }
  
  else if (req.url.endsWith('.js.gz')) {
    res.set('Content-Encoding', 'gzip');
    res.set('Content-Type', 'application/javascript');
  } else if (req.url.endsWith('.wasm.gz')) {
    res.set('Content-Encoding', 'gzip');
    res.set('Content-Type', 'application/wasm');
  } else if (req.url.endsWith('.data.gz')) {
    res.set('Content-Encoding', 'gzip');
    res.set('Content-Type', 'application/octet-stream');
  }
  
  next();
});

app.use(express.static(path.join(__dirname, 'dist')));

app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, 'dist', 'index.html'));
});

app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});
