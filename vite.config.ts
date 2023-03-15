/* eslint-disable import/no-extraneous-dependencies */
import path from 'path';

import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
  root: path.resolve(__dirname, 'frontend'),
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        xfwd: true,
        changeOrigin: false,
        secure: false,
        rewrite: (subdir) => subdir.replace(/^\/api/, ''),
        ws: true, // even if true, websocket proxy doesn't work.
      },
    },
  },
});
