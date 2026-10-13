# construct-010-trials

Trial workspaces for the CONSTRUCT-010 study ("Reduce Construction Cost
Without Sacrificing Correctness"). Study records live in
kemiller2002/research-documents, `research/construction-by-design/CONSTRUCT-010/`.

This repository must contain **only**:

- this README on `main`;
- orphan run branches `trial/<slice>/<arm>/<run>`, one per trial, published by
  the host. Each holds only what that trial's agent may see: the permitted
  sources and, for arms C2 and A+, `HANDOFF.md` and `outbox/`. The trial
  session's own pushes add `outbox/` changes and `requests/*.json`; the host
  relay adds `responses/*.json`.

Never add requirements oracles, acceptance suites, ECIR, construction plans,
prepared artifacts, digests, caches or prior results here. A trial session
can read every branch of this repository.
