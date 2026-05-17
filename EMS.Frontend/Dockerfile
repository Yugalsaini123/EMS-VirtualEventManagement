# Multi-stage build for EMS.Frontend
# Stage 1: Build
FROM node:20-alpine AS builder

WORKDIR /app

# Copy package files
COPY EMS.Frontend/package*.json ./

# Install dependencies
RUN npm ci --only=production && npm ci --only=development

# Copy application source
COPY EMS.Frontend/ .

# Build the application
RUN npm run build

# Stage 2: Runtime - Nginx
FROM nginx:alpine

# Install curl for health checks
RUN apk add --no-cache curl

# Create app directory
WORKDIR /usr/share/nginx/html

# Copy Nginx configuration
COPY EMS.Frontend/nginx.conf /etc/nginx/nginx.conf

# Copy built application from builder
COPY --from=builder /app/dist/EMS.Frontend/browser /usr/share/nginx/html

# Create assets directory if it doesn't exist
RUN mkdir -p /usr/share/nginx/html/assets

# Create a default config.json with placeholder
RUN echo '{"apiUrl":"http://ems-api-28551.centralindia.azurecontainer.io:5000/api/v1"}' > /usr/share/nginx/html/assets/config.json

# Remove default nginx config
RUN rm -f /etc/nginx/conf.d/default.conf

# ✅ Fix runtime folder permissions - must be done as root before USER nginx
RUN mkdir -p /var/run/nginx /var/cache/nginx /var/log/nginx && \
    chown -R nginx:nginx /usr/share/nginx/html /etc/nginx /var/log/nginx /var/run /var/cache/nginx && \
    chmod 755 /var/run/nginx /var/cache/nginx

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl --fail http://localhost:80/ || exit 1

# Switch to non-root user
# USER nginx

# Run Nginx in foreground (single CMD only)
CMD ["nginx", "-g", "daemon off;"]
