# docker build -t php_devserver . && docker run -p 443:443 -v "C:\Github\Wmc\50_PhpWithVue\htdocs:/var/www/html" -it --rm php_devserver
# https://github.com/TrafeX/docker-php-nginx
ARG ALPINE_VERSION=3.18
FROM alpine:${ALPINE_VERSION}
LABEL Maintainer="Tim de Pater <code@trafex.nl>"
LABEL Description="Lightweight container with Nginx 1.22 & PHP 8.2 based on Alpine Linux."
# Setup document root
WORKDIR /var/www/html

# Install packages and remove default server definition
RUN apk add --no-cache \
  curl \
  nginx \
  php82 \
  php82-ctype \
  php82-curl \
  php82-dom \
  php82-fpm \
  php82-gd \
  php82-intl \
  php82-mbstring \
  php82-opcache \
  php82-openssl \
  php82-pdo_mysql \
  php82-pdo_sqlite \
  php82-phar \
  php82-session \
  php82-xml \
  php82-xmlreader \
  supervisor

# Configure nginx - http
COPY config/nginx.conf /etc/nginx/nginx.conf
# Configure nginx - default server
COPY config/conf.d /etc/nginx/conf.d/
RUN mkdir -p /etc/ssl
COPY certs/server.* /etc/ssl/

# Configure PHP-FPM
COPY config/fpm-pool.conf /etc/php82/php-fpm.d/www.conf
COPY config/php.ini /etc/php82/conf.d/custom.ini

# Configure supervisord
COPY config/supervisord.conf /etc/supervisor/conf.d/supervisord.conf

# Make sure files/folders needed by the processes are accessable when they run under the nobody user
RUN chown -R nobody.nobody /etc/ssl /var/www/html /run /var/lib/nginx /var/log/nginx

# Switch to use a non-root user from here on
USER nobody

# Add application
RUN echo "<?php phpinfo(); ?>" > /var/www/html/index.php

# Expose the port nginx is reachable on
EXPOSE 443

# Let supervisord start nginx & php-fpm
CMD ["/usr/bin/supervisord", "-c", "/etc/supervisor/conf.d/supervisord.conf"]

# Configure a healthcheck to validate that everything is up&running
HEALTHCHECK --timeout=10s CMD curl --silent --fail http://127.0.0.1:8080/fpm-ping
