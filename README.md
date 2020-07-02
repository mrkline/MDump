# Retrospective, July 2020

MDump was a project to merge multiple images into a single one
to more rapidly upload them to an image board, i.e., "dump" them.
Plenty of silly, newbie stuff, but also some interesting bits:

- Work is done in a separate thread,
  with status updates marshalled over to the UI one

- Binary space partitioning to merge the input images into a single mosaic

- Metadata is embedded into the output image to split it back up

- My first foray into popular C libraries like libpng
  (lol `setjmp` exception handling)

- FFI with P/Invoke from C#
