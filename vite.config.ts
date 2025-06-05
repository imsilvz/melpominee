/* eslint-disable import/no-extraneous-dependencies */
import path from 'path';

import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
  css: {
    preprocessorOptions: {
      scss: {
        api: 'modern-compiler',
        silenceDeprecations: ["legacy-js-api"],
      }
    }
  },
  root: path.resolve(__dirname, 'frontend'),
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        //target: 'https://melpominee.app/',
        xfwd: true,
        //changeOrigin: true,
        secure: false,
        rewrite: (subdir) => subdir.replace(/^\/api/, ''),
        ws: true, // even if true, websocket proxy doesn't work.
      },
    },
  },
});
