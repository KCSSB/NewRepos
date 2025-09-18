FROM node:18

WORKDIR /app

COPY package.json package-lock.json ./

RUN npm install esbuild@0.25.4 && npm ci

COPY . .

EXPOSE 5173

CMD ["npm", "run", "dev", "--", "--host"]
