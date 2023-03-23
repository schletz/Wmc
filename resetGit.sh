#!/bin/bash

echo "Warning: All local changes will be reset. Press CTRL+C to cancel."
read -p "Press [Enter] to continue"

# Get the current branch
current_branch=$(git branch --show-current)

# Check if there are any uncommitted changes
if [ -n "$(git status --porcelain)" ]; then
    echo "You have uncommitted changes. Please commit or stash them before running this script."
    exit 1
fi

# Reset all branches
for branch in $(git branch); do
    echo "Resetting branch $branch..."
    git checkout $branch && git clean -df && git reset --hard "origin/$branch"
    if [ $? -ne 0 ]; then
        echo "Failed to reset branch $branch."
        exit 1
    fi
done

# Go back to the current branch
git checkout $current_branch
echo "You are now on branch $current_branch"
