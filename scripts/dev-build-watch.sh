#!/bin/bash

# check and clear processes using the expected ports
fuser 3003/tcp 2>/dev/null && fuser -k 3003/tcp
fuser 4004/tcp 2>/dev/null && fuser -k 4004/tcp
fuser 5005/tcp 2>/dev/null && fuser -k 5005/tcp
fuser 6006/tcp 2>/dev/null && fuser -k 6006/tcp

npm run dev:build-watch