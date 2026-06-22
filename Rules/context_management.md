# Large Files & Context Management

- When editing a large file (>300 lines), first ask to see specific sections by having me paste them or using `@` mentions.
- In Agent mode, use `search_content` tool to locate code before suggesting edits. Do not assume line numbers.
- Do not output the entire file if you are only changing one function. Output only the function or the changed block with context markers.
- If a file is minified or generated, warn me before attempting to edit it.