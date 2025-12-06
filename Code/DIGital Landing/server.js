const express = require('express');
const path = require('path');

const app = express();
const PORT = process.env.PORT || 3000;

// Middleware to set proper headers for Brotli-compressed Unity WebGL files
app.use((req, res, next) => {
  // Handle .br files (Brotli compressed)
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
  
  // Handle .gz files (Gzip compressed) - in case you have any
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

// Serve static files from the dist directory
app.use(express.static(path.join(__dirname, 'dist')));

// Handle React Router - serve index.html for all routes
app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, 'dist', 'index.html'));
});

app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});
