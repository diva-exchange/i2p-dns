FROM node:alpine as base

WORKDIR /

COPY package.json yarn.lock ./

RUN rm -rf node_modules && yarn install --frozen-lockfile && yarn cache clean && npm run build

COPY . .

#RUN npm run build

CMD ["node", "./build/app.js"]