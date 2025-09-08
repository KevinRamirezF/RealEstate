# Use Node.js 20 LTS for building (better npm compatibility)
FROM node:20-alpine AS build-env
WORKDIR /app

# Copy package.json
COPY package.json ./

# Clean install with latest npm and no cache
RUN npm cache clean --force && \
    npm install --no-optional --prefer-offline=false

# Copy source code
COPY . .

# Build the documentation
RUN npm run build

# Use nginx to serve the static files
FROM nginx:alpine

# Copy the built documentation
COPY --from=build-env /app/build /usr/share/nginx/html

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Create a non-root user
RUN addgroup -g 1001 -S appgroup && \
    adduser -S appuser -G appgroup && \
    chown -R appuser:appgroup /usr/share/nginx/html && \
    chown -R appuser:appgroup /var/cache/nginx && \
    chown -R appuser:appgroup /var/log/nginx && \
    chown -R appuser:appgroup /etc/nginx/conf.d && \
    touch /var/run/nginx.pid && \
    chown -R appuser:appgroup /var/run/nginx.pid

USER appuser

# Expose port 8080
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/ || exit 1

# Start nginx
CMD ["nginx", "-g", "daemon off;"]