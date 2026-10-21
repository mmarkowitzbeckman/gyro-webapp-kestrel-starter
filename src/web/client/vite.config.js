import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import { VitePWA } from "vite-plugin-pwa";

// The GraphQL API runs as a separate HTTPS server (see src/Server/Program.cs). In dev we
// proxy /graphql to it so the browser only ever talks to the Vite origin — no CORS
// and no need to accept the API's self-signed cert separately. In production the
// Kestrel web server (src/Server/Web/WebApp.cs) performs the equivalent proxy.
const API_HTTPS_PORT = process.env.API_HTTPS_PORT || 5005;

// https://vite.dev/config/
export default defineConfig({
  base: "/kestrel-example/",
  plugins: [
    react(),
    VitePWA({
      registerType: "autoUpdate",
      includeAssets: ["favicon.svg"],
      manifest: {
        name: "Gyroscope Webapp Starter",
        short_name: "Gyroscope",
        description: "Gyroscope webapp starter",
        theme_color: "#111111",
        background_color: "#111111",
        display: "standalone",
        icons: [
          {
            src: "pwa-192x192.svg",
            sizes: "192x192",
            type: "image/svg+xml",
          },
          {
            src: "pwa-192x192.svg",
            sizes: "512x512",
            type: "image/svg+xml",
          },
          {
            src: "pwa-192x192.svg",
            sizes: "512x512",
            type: "image/svg+xml",
            purpose: "maskable",
          },
        ],
      },
      workbox: {
        globPatterns: ["**/*.{js,css,html,svg,png,woff2}"],
        runtimeCaching: [
          {
            urlPattern: /\/graphql/,
            handler: "NetworkFirst",
            options: {
              cacheName: "api-cache",
              expiration: { maxEntries: 50, maxAgeSeconds: 300 },
              networkTimeoutSeconds: 3,
            },
          },
        ],
      },
    }),
  ],
  server: {
    proxy: {
      "/kestrel-example/graphql": {
        target: `https://localhost:${API_HTTPS_PORT}`,
        changeOrigin: true,
        secure: false, // API uses a self-signed cert in dev
        rewrite: (path) => path.replace(/^\/kestrel-example/, ""),
      },
    },
  },
});
