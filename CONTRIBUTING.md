# Branch naming for members:
Pushing to main is disabled and can only be done through a PR.
Please follow the following branc naming conventions before creating your branch:
- **feature/\<name>** for new features
- **chore/\<name>** for non feature changes/improvements
- **fix/\<name>** for fixes
- **hotfix/\<name>** for urgent simple fixes

Merging is also disabled with linear history enabled. That means that only squash or rebase is enabled as PR merge methods.
Follow this conventions when deciding between squash or rebase:
- **Squash:** If your commit history is not particularly pretty and/or confusing i.e. lots o wip, fix, hotfix or revert commits.
- **Merge:** If you strictly followed with the commit standards described bellow.

# Commit standards:
Commits at this repo are heavily inspired by https://gitmoji.dev/ and https://conventionalcommits.org/

The formating is as follows:
- \<intention> \<scope>: \<message>

Where:
- **Intent** is an emoji that can be found at: https://gitmoji.dev/
- **Scope** is one of: 'feature', 'fix', 'hotfix' or 'chore'
- **Message** is a message that describes the reasoning behind your commit

> [!NOTE]
> When writing your message try to convey what the reason behind those changes is and not what the change is.
> 
> You don't need to explain WHY just what you are doing beyond the simple "created x file".
>
> Also: use verbs in the imperative form.
>
> **Example:**
> - Write: "create persistence for match entity/data"
> - Instead of: "added MatchEntity"

General commit examples:
- ✨ feat: migrate to bun 1.2.4
- ⬆️ chore: upgrade dependencies
- 🐛 fix: add required defaults on prisma client
- 🚑️ hotfix: add tzdata to production dockerfile
- 🔀 chore: merge from main

As you can see, most commits are going to fall into those three scopes: feat, chore or fix

You can use other commit scopes where necessary but those specifications are generally best used when 
limited to a simple set of options.

This is what **I** follow:
- **feat:** new feature is added to the code, something that it didn't do before
- **chore:** anything related to code maintenance not directly related to a feature or a fix (includes merges for example)
- **fix:** changes related to misbehaving code
- **hotfix:** simple fix done without extensive testing for urgent matters

At the emoji part, feel free to use any emoji listed [here](https://gitmoji.dev/).
The emoji part of the commit is less important and is more of a simpler way to convey your message without fully reading it.

> [!IMPORTANT]
> Please use the Unicode emoji and not the shortcode. Take a look at https://gitmoji.dev/specification for pros and cons
> 
> Ex: "⬆️ chore: upgrade dependencies" instead of  ":arrow_up: chore: upgrade dependencies"

If i havent convinced you yet, think for example of someone searching for a specific chore commit where files where moved or renamed:
It is way easier to identify a commit named:
- 🚚 chore(TDI-123): move/rename foo.ts to bar.ts

than it is a commit named:
- chore(TDI-123): move/rename foo.ts to bar.ts

# Testing:
No test coverage is enforced but please make sure to write enough tests where necessary, especially when dealing with external services/integrations.

[Dotnet Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/testing/write-your-first-test?pivots=xunit) makes it very easy to create integration tests and make sure everything runs smoothly
