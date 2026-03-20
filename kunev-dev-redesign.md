# kunev.dev — Репозиционирање и Редизайн

## Контекст и цел

Текущият сайт е generic портфолио с позиция "C# and ASP.NET Core MVC development". Целта е да се трансформира в **авторитетна платформа** около AI интеграция в образованието — нишата, в която Yavor Kunev реално работи (докторска дисертация за Intelligent Tutoring Systems, преподавател по C#, разработчик на CodeGrade).

**Нова позиция:**
> *"AI integration for education — research, tools & consulting"*

---

## Нова структура на сайта

### Навигация
```
Home | Writing | Research | Projects | Consulting | Contact
```

---

### 1. Hero секция (Home)

**НЕ** "Hi, I'm a developer with expertise in C#."

**ДА** — ясна стойностна пропозиция:

```
headline:    "AI that teaches, not just answers."
subheadline: "I build intelligent tools for programming education —
              grounded in cognitive science, designed for real classrooms."
CTA buttons: [See My Work]  [Let's Talk]
```

- Визуално: минималистично, тъмна тема, акцент с топъл amber/gold цвят
- Без снимка на главната — или малка, неотрупваща
- Едно изречение за credibility: *"Doctoral researcher in AI-assisted learning. Creator of CodeGrade."*

---

### 2. About (кратко, inline на Home)

Преработен текст — фокус върху intersection AI + education, не просто tech stack:

```
I'm a software engineer and doctoral researcher exploring how AI can make 
programming education more effective — not just more automated.

My work sits at the intersection of cognitive load theory, intelligent 
tutoring systems, and the practical realities of teaching code to beginners.

When I'm not researching, I build tools that put these ideas into practice.
```

**НЕ включва:** "passionate", "love for clean code", generic buzzwords.

---

### 3. Projects (преработен)

Не списък от проекти — **case studies** с narrative:

За всеки проект:
```
Проблем → Подход → Резултат → Технологии → [Live Demo / GitHub]
```

**CodeGrade** трябва да е featured project #1:
```
title:    CodeGrade — Automated Code Assessment for Programming Education
problem:  Teachers spend hours grading student code. Feedback arrives too late 
          to change learning outcomes.
approach: Built a Judge0-powered platform with role-based access for students 
          and teachers. Next: AI feedback layer for pedagogical commentary.
result:   [статистики от реалната употреба, ако има]
stack:    C# / ASP.NET Core / Judge0 API / PostgreSQL
links:    [codegrade.kunev.dev] [GitHub]
```

**schoolselect.net** — ако искаш да го включиш:
```
title:    SchoolSelect — Secondary School Comparison for Bulgarian Families
problem:  Parents navigate complex НВО admission scores without clear tooling.
approach: Platform aggregating school profiles, admission scores, and program 
          info for data-driven school choice.
stack:    [stack]
```

---

### 4. Writing (Блог)

Статична страница + списък с posts. Дори без posts сега — страницата трябва да съществува с 1-2 draft заглавия, за да покаже посоката.

**Предложени първи статии (заглавия):**
1. *"Why ChatGPT Makes a Bad Programming Tutor — and What Good AI Tutoring Looks Like"*
2. *"Running a Local AI Server for Research: What I Learned"*
3. *"Cognitive Load Theory Meets LLMs: Are We Teaching Programming Wrong?"*

---

### 5. Research

Кратка страница за дисертацията — достъпно написана, не академичен abstract:

```
heading:  Current Research
topic:    Intelligent Tutoring Systems for Programming Education in the LLM Era
context:  Doctoral research at [университет]
summary:  As LLMs reshape how people learn to code, what cognitive competencies 
          should AI tutoring systems actually develop? My research examines this 
          through the lens of Cognitive Load Theory and the ICAP framework.
status:   In progress — [expected completion]
```

---

### 6. Consulting

Ясна страница с какво предлагаш:

```
heading: Work With Me

I help educational institutions, EdTech companies, and research teams:

→ Integrate AI tools into learning platforms (not just ChatGPT wrappers)
→ Design pedagogically grounded feedback systems for coding education  
→ Set up local AI infrastructure for research and institutional use
→ Advise on ITS architecture and AI-assisted assessment

[Book a Discovery Call]  ←  линк към Calendly или contact form
```

---

### 7. Contact

Опростен — само:
- Email: yavor@kunev.dev
- GitHub
- LinkedIn
- Форма (опционална)

---

## Дизайн насоки

### Тема и цвят
- **Dark theme** — тъмно синьо/charcoal background, не чисто черно
- **Акцент:** топъл amber `#F59E0B` или electric teal `#2DD4BF` — избери едно
- **Текст:** off-white `#F8FAFC`, не чисто бяло

### Типография
- **Display/Headlines:** Fraunces, Playfair Display, или Syne — характерни, не generic
- **Body:** DM Sans, Plus Jakarta Sans, или Instrument Sans
- **Mono (за code snippets):** JetBrains Mono или Fira Code

### Layout принципи
- Generous whitespace — не претрупано
- Лека асиметрия в hero секцията
- Subtle grid или dot pattern на background-а
- Hover animations на project cards — slide-up reveal на допълнителен текст

### Какво да СЕ ИЗБЯГВА
- Лилави градиенти
- Inter / Roboto / Arial
- "Passionate developer" копи
- Generic card grid без narrative
- Снимки на laptop с код на екрана

---

## Технически изисквания

- Запази съществуващата ASP.NET Core MVC структура
- Само frontend промени (HTML/CSS/JS в Razor views)
- Mobile responsive — mobile-first подход
- Добави `<meta>` Open Graph тагове за social sharing
- Добави structured data (JSON-LD) за Person schema

---

## Приоритет на имплементация

```
1. Hero секция — нов headline, subheadline, CTA (30 мин)
2. About текст — преработен копи (15 мин)
3. Projects → Case studies формат (45 мин)
4. Consulting страница — нова (30 мин)
5. Research страница — нова (20 мин)
6. Writing/Blog страница — placeholder + 3 заглавия (15 мин)
7. Визуален редизайн — цветове, шрифтове, dark theme (60-90 мин)
8. Meta tags + JSON-LD (15 мин)
```

---

## Tone of Voice

- **Директен**, не самохвалебствен
- **Confident**, не арогантен  
- **Технически компетентен**, но достъпен
- **Research-backed** — позоваването на frameworks (CLT, ICAP) е плюс, не jargon
- Пиши на **английски** (международна аудитория)
