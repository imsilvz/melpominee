FROM nginx AS base
WORKDIR /app

FROM node:lts AS build
COPY . /src
WORKDIR /src
RUN npm i --prefix src/frontend
RUN npm run build

FROM base AS final
WORKDIR /usr/share/nginx/html/
COPY --from=build /src/src/frontend/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /src/src/frontend/dist .
RUN ls
