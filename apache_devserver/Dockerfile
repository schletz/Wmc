FROM ubuntu:22.04 AS builder

# Stage 1 ******************************************************************************************
# Compiling SQL Server drivers
RUN ln -snf /usr/share/zoneinfo/UTC /etc/localtime
RUN echo UTC > /etc/timezone

RUN apt-get update && apt-get -y install unixodbc-dev php8.1 php8.1-dev php-pear

# SQL Server ODBC Driver
# Additional package sources
# https://learn.microsoft.com/en-us/sql/connect/odbc/linux-mac/installing-the-microsoft-odbc-driver-for-sql-server?view=sql-server-ver16#ubuntu18
RUN curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add -
RUN curl https://packages.microsoft.com/config/ubuntu/22.04/prod.list > /etc/apt/sources.list.d/mssql-release.list
RUN apt-get update && ACCEPT_EULA=Y apt-get -y install msodbcsql18

# Compile PHP modules
# From https://askubuntu.com/questions/1407533/microsoft-odbc-v18-is-not-find-by-apt
RUN pecl install pdo_sqlsrv
RUN pecl install sqlsrv

# Stage 2 ******************************************************************************************
# Final image
FROM ubuntu:22.04 

RUN ln -snf /usr/share/zoneinfo/UTC /etc/localtime
RUN echo UTC > /etc/timezone

RUN apt-get update && apt-get -y install curl openssl unixodbc zip \
    php8.1 php8.1-bcmath php8.1-curl php-json php8.1-mbstring php8.1-xml php8.1-zip 

# Copy compiled SQL Server modules from stage 1
COPY --from=builder /usr/lib/php/20210902/sqlsrv.so /usr/lib/php/20210902/sqlsrv.so
COPY --from=builder /usr/lib/php/20210902/pdo_sqlsrv.so /usr/lib/php/20210902/pdo_sqlsrv.so
COPY --from=builder /etc/odbcinst.ini /etc/odbcinst.ini
COPY --from=builder /opt/microsoft/msodbcsql18 /opt/microsoft/msodbcsql18
RUN printf "; priority=20\nextension=sqlsrv.so\n" > /etc/php/8.1/mods-available/sqlsrv.ini
RUN printf "; priority=30\nextension=pdo_sqlsrv.so\n" > /etc/php/8.1/mods-available/pdo_sqlsrv.ini
RUN phpenmod -v 8.1 sqlsrv pdo_sqlsrv

# Laravel composer
RUN curl https://raw.githubusercontent.com/composer/getcomposer.org/76a7060ccb93902cd7576b67264ad91c8a2700e2/web/installer | php --
RUN mv composer.phar /usr/local/bin/composer

COPY virtualhost.conf /etc/apache2/sites-available/000-default.conf
COPY php.ini /etc/php/8.1/apache2/php.ini

VOLUME /var/www/html
EXPOSE 80 443 8000

COPY compose_app.sh /
RUN chmod a+x ./compose_app.sh
COPY sql_server_test.php /
ENTRYPOINT /compose_app.sh
