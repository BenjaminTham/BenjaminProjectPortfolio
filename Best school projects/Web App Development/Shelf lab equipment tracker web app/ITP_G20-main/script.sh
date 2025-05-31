#!/bin/bash

# Run npm install
npm i

# Run Prisma migrate
npx prisma migrate deploy

# Run npm build
npm run build

# Run npm start
npm start