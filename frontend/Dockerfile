FROM nginx AS base
WORKDIR /app

FROM node:lts AS build
COPY . /src
WORKDIR /src
RUN npm i
RUN npm run build

FROM base AS final
WORKDIR /usr/share/nginx/html/
COPY --from=build /src/frontend/dist .
RUN ls