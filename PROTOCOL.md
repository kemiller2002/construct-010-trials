# Run protocol

Edit files in `outbox/` only, by the flat file names HANDOFF.md permits, plus `outbox/dispositions.json`.
To ask the host for something, commit the outbox together with one request file and push this branch:

- `requests/try-NNN.json` containing `{}`: the host applies the outbox and builds it.
- `requests/check-NNN.json` containing `{"check":"<id>"}`: the host applies the outbox and runs that approved check.
- `requests/submit-NNN.json` containing `{}`: the host applies the outbox as your submission. Submit once.

NNN is a three-digit sequence (001, 002, ...). The host answers in `responses/<same name>`; pull to read it.
A response says passed, failed, refused, unauthorizedWrite, timedOut, crashed or submitted, with output tails.
Responses are feedback only, not acceptance.
