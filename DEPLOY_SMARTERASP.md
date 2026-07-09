# Автоматичен деплой на ASP.NET Core към SmarterASP.NET с GitHub Actions

**Push-to-deploy playbook.** При `git push` към `main` → GitHub Actions билдва проекта (`dotnet publish`) и го качва по FTP на SmarterASP. Секретите остават на сървъра, извън git.

> Този файл е генеричен — копирай го в кой да е ASP.NET Core проект на SmarterASP и следвай стъпките. Примерите са от проекта CodeGrade (`codegrade.kunev.dev`).

---

## 0. Прочети първо: какво НЕ работи (и защо)

SmarterASP има панел **„Auto Build and Deploy" → GitHub Deploy** с бутон **Environment Variables**. **Не го използвай за .NET:**

- Този деплой минава през **инфраструктурата за Node.js App** (виж KB a2335 — стъпките казват „Select the Node.js App option").
- **Env vars от този панел НЕ стигат до .NET app pool-а.** (Доказано на живо: приложението четеше `appsettings.Production.json` от сървъра, а env var-ът се игнорираше — въпреки че в ASP.NET Core env vars имат по-висок приоритет от JSON.)
- Деплоят е **ръчен** („Deploy Now"), не автоматичен при push; не чисти стари файлове.

**Изводът:** за ASP.NET Core използвай **GitHub Actions + FTP** (този playbook), а секретите дръж в **`appsettings.Production.json` само на сървъра**.

---

## 1. Предпоставки

- ASP.NET Core проект в **GitHub** repo (публично или частно — работи и с двете).
- Хостинг на **SmarterASP.NET** с работещ сайт (in-process IIS хостинг).
- **FTP данни** за сайта (хост, потребител, парола).
- Инсталиран .NET runtime на сървъра, съвпадащ с `TargetFramework` на проекта (SmarterASP поддържа .NET 6/7/8/9).

---

## 2. Стъпка 1 — Вземи FTP данните от SmarterASP

От контролния панел (или от Visual Studio publish профил, ако имаш) ти трябват:

| Какво | Пример (CodeGrade) | Къде |
|---|---|---|
| FTP хост | `win1039.site4now.net` | Websites → FTP, или welcome имейл |
| FTP потребител | `petrovkunev-001` | същото |
| FTP парола | (тайна) | същото |
| Физически път на сайта | `h:\root\home\petrovkunev-001\www\codegrade\` | File Manager (за ориентир) |

> Ако имаш Visual Studio FTP publish профил (`Properties/PublishProfiles/*.pubxml`), там вече са `publishUrl`, `UserName` и `FtpSitePath` — удобен източник.

---

## 3. Стъпка 2 — Добави GitHub Secrets

В repo-то: **Settings → Secrets and variables → Actions → New repository secret**
(директно: `https://github.com/<USER>/<REPO>/settings/secrets/actions`)

Създай точно тези три (имена само с букви/цифри/долни черти, без интервали):

| Name | Стойност |
|---|---|
| `FTP_SERVER` | само хостът, напр. `win1039.site4now.net` (без `ftp://`, без наклонена черта) |
| `FTP_USERNAME` | FTP потребителят |
| `FTP_PASSWORD` | FTP паролата |

GitHub автоматично маскира тези стойности в логовете на Actions.

---

## 4. Стъпка 3 — Тайни и конфигурация (само на сървъра)

Тайните (connection string, API ключове, SMTP парола) живеят в **`appsettings.Production.json` на сървъра**, а НЕ в git.

1. `.gitignore` трябва да игнорира `appsettings*.json` (за да не влязат тайни в repo-то).
2. През File Manager създай/редактирай `appsettings.Production.json` в кореновата папка на сайта. Пример:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=SQLxxxx.site4now.net;Initial Catalog=db_xxx;User Id=db_xxx_admin;Password=SAMO_BUKVI_I_CIFRI;MultipleActiveResultSets=true;TrustServerCertificate=true"
  },
  "EmailSettings": { "SmtpPassword": "..." },
  "AllowedHosts": "твоят-домейн"
}
```

> **DB паролата — само букви и цифри.** Специални символи (`# ; = % интервал` и кавички) чупят connection string-а или `.env` парсери. (`#` например се тълкува като коментар в `.env`.)

**CI НЕ пипа тези файлове** — workflow-ът долу изключва `appsettings*.json` при синхронизацията, така че сървърният конфиг оцелява при всеки деплой.

---

## 5. Стъпка 4 — Изрична среда в `web.config`

За да е детерминирано, че приложението е в Production (lowercase URLs, HSTS и т.н.), задай средата изрично. Това е и **надеждният** начин да подаваш env vars на .NET под IIS (за разлика от панела) — но **само за не-тайни**, защото `web.config` е в git.

```xml
<aspNetCore processPath="dotnet" arguments=".\ТвойПроект.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess">
  <environmentVariables>
    <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
  </environmentVariables>
</aspNetCore>
```

---

## 6. Стъпка 5 — Намери точната FTP папка (`REMOTE_DIR`)

Критично: трябва да знаеш **в коя папка те пуска FTP** спрямо кореновата папка на сайта. Има три чести случая:

- FTP те пуска **директно в site root** (там са `web.config`, `<Проект>.dll`, `wwwroot/`) → `REMOTE_DIR: '.'`
- FTP те пуска **едно ниво нагоре** и сайтът е в подпапка → `REMOTE_DIR: 'име-на-подпапката'`
- По-дълбоко → `REMOTE_DIR: 'www/сайт'` и т.н.

**Не гадай — виж го.** Добави временно тази стъпка в workflow-а (преди същинския деплой), пусни веднъж и виж изхода:

```yaml
      - name: List FTP structure (temporary diagnostic)
        run: |
          lftp -c "
          set ftp:ssl-allow no
          open -u '${{ secrets.FTP_USERNAME }}','${{ secrets.FTP_PASSWORD }}' '${{ secrets.FTP_SERVER }}'
          echo '=== PWD ==='; pwd
          echo '=== listing ==='; cls -l
          " || true
```

Ако в листинга видиш директно `web.config` и `<Проект>.dll` → `REMOTE_DIR: '.'`. Ако видиш подпапка (напр. `www` или името на сайта) → влез в нея с още едно `cls -l www` и т.н. **Махни тази стъпка**, щом фиксираш пътя.

---

## 7. Стъпка 6 — Workflow файлът (шаблон за копиране)

Сложи го в `.github/workflows/deploy.yml`. **Единствените неща за смяна** са `DOTNET_VERSION` (спрямо `TargetFramework`) и `REMOTE_DIR` (от Стъпка 5). Ако repo-то има повече от един проект, добави пътя до `.csproj` в реда `dotnet publish`.

```yaml
name: Deploy to SmarterASP.NET

# Автоматичен деплой при push към main. Може и ръчно от таба Actions.
on:
  push:
    branches: [ main ]
  workflow_dispatch:

# Не пускай два деплоя едновременно.
concurrency:
  group: production-deploy
  cancel-in-progress: false

env:
  DOTNET_VERSION: '8.0.x'   # ← спрямо TargetFramework (net8.0 → 8.0.x, net9.0 → 9.0.x)
  REMOTE_DIR: '.'           # ← FTP папката на сайта (Стъпка 5). '.' = FTP те пуска в site root.

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Restore
        run: dotnet restore

      - name: Publish
        run: dotnet publish -c Release -o ./publish --no-restore
        # Ако имаш няколко проекта: dotnet publish ПЪТ/ДО/Проект.csproj -c Release -o ./publish --no-restore

      # Страница "в поддръжка": качва се ПЪРВА, за да спре приложението и да отключи .dll
      # (in-process хостингът ги държи заключени, докато сайтът работи).
      - name: Create app_offline.htm
        run: echo '<html><head><meta charset="utf-8"></head><body style="font-family:sans-serif;text-align:center;padding-top:3rem"><h1>Обновяваме сайта…</h1><p>Ще е достъпен след минута.</p></body></html>' > app_offline.htm

      - name: Install lftp
        run: sudo apt-get update && sudo apt-get install -y lftp

      # Първо спри приложението: качи app_offline.htm и ИЗЧАКАЙ IIS да освободи
      # заключените .dll. In-process хостингът ги пуска с няколко секунди закъснение —
      # ако mirror тръгне веднага, първите ъплоуди удрят заключен файл и деплоят пада.
      # (Доказано на живо: KunevDevPortfolio, run #3.)
      - name: Stop app (upload app_offline.htm)
        run: |
          lftp -c "
          set cmd:fail-exit yes
          set ftp:ssl-allow no
          set net:timeout 15
          set net:max-retries 3
          open -u '${{ secrets.FTP_USERNAME }}','${{ secrets.FTP_PASSWORD }}' '${{ secrets.FTP_SERVER }}'
          put -O '${{ env.REMOTE_DIR }}' app_offline.htm
          "
          echo "Waiting for IIS to release file locks..."
          sleep 15

      # Синхронизирай → изтрий app_offline (сайтът се вдига).
      #  --no-perms  : SmarterASP FTP НЕ поддържа SITE CHMOD (иначе chmod грешки чупят деплоя).
      #  --delete    : чисти остарели файлове, за да съвпада сървърът с билда.
      #  -X 'appsettings*.json' + --exclude 'logs/' : НИКОГА не пипа конфига/тайните/логовете на сървъра.
      - name: Deploy over FTP
        run: |
          set -o pipefail
          lftp -c "
          set cmd:fail-exit yes
          set ftp:ssl-allow no
          set net:timeout 15
          set net:max-retries 3
          set net:reconnect-interval-base 5
          open -u '${{ secrets.FTP_USERNAME }}','${{ secrets.FTP_PASSWORD }}' '${{ secrets.FTP_SERVER }}'
          mirror -R --no-perms --delete --parallel=4 --verbose -X app_offline.htm -X 'appsettings*.json' --exclude 'logs/' ./publish/ '${{ env.REMOTE_DIR }}'
          rm -f '${{ env.REMOTE_DIR }}/app_offline.htm'
          " 2>&1 | tee deploy.log

      # Логът в job summary — удобен за преглед в Actions UI без ровене в пълния лог.
      - name: Publish deploy log to job summary
        if: always()
        run: |
          {
            echo '### Deploy log (tail)'
            echo '```'
            tail -c 20000 deploy.log 2>/dev/null || echo 'no deploy.log'
            echo '```'
          } >> "$GITHUB_STEP_SUMMARY"

      # Предпазна мрежа: маха app_offline.htm при ВСЯКАКЪВ изход, за да не остане сайтът "в поддръжка".
      - name: Ensure site is back online
        if: always()
        run: |
          lftp -c "
          set ftp:ssl-allow no
          open -u '${{ secrets.FTP_USERNAME }}','${{ secrets.FTP_PASSWORD }}' '${{ secrets.FTP_SERVER }}'
          rm -f '${{ env.REMOTE_DIR }}/app_offline.htm'
          " || true
```

---

## 8. Стъпка 7 — Първо пускане (безопасен ред)

1. **Първия път махни `--delete`** от реда `mirror` (за да не изтрие нищо, ако `REMOTE_DIR` е сбъркан). При грешен път файловете просто отиват другаде — нищо не се трие.
2. Commit + push (или пусни ръчно от таба Actions чрез `workflow_dispatch`).
3. Гледай run-а в **Actions**. Провери, че сайтът се обновява и работи.
4. Щом е потвърдено — **върни `--delete`** и push пак. Оттук нататък имаш чист push-to-deploy.

> Заради `concurrency` + `cancel-in-progress: false` няколко бързи push-а се изпълняват **един по един**. Гледай винаги **най-горния** (най-нов) run.

---

## 9. Отстраняване на проблеми (наученото на живо)

| Симптом в лога / браузъра | Причина | Решение |
|---|---|---|
| `Login failed for user … (18456)` | Паролата в connection string ≠ реалната DB парола | Сложи точната парола в `appsettings.Production.json`; направи я само букви/цифри |
| `cd: Access failed: 550 … syntax is incorrect` | Грешен `REMOTE_DIR` | Пусни диагностиката от Стъпка 5 и виж реалния път |
| `chmod: … SITE CHMOD are not supported` + exit 1 | SmarterASP FTP няма SITE CHMOD | Добави `--no-perms` към `mirror` |
| `mirror` пада веднага след качване на `app_offline.htm` | IIS още не е освободил заключените `.dll` (in-process) | Отделен step за `app_offline.htm` + `sleep 15` преди `mirror` (в шаблона по-горе) |
| Празен commit не тригерира деплой | `paths-ignore` изисква поне един променен не-`.md` файл | Пусни ръчно от Actions (`workflow_dispatch`) или промени файл |
| Сайтът стои на „в поддръжка" | `app_offline.htm` е останал (деплоят е паднал преди `rm`) | Стъпката `if: always()` го маха; или изтрий `app_offline.htm` ръчно |
| Промените в env vars нямат ефект | Панелът на SmarterASP не храни .NET | Пиши в `appsettings.Production.json` на сървъра |
| Сайтът пада след смяна на парола | Старият `appsettings.Production.json` на сървъра още има старата | Обнови файла на сървъра с новата стойност |
| Искаш да видиш реалната грешка | Продукцията крие детайлите | В `web.config` временно `stdoutLogEnabled="true"`, рестарт, виж `logs\stdout_*.log`, после върни на `false` |

**Диагностичен трик за база данни (извън приложението):**
```
sqlcmd -S SQLxxxx.site4now.net -U db_xxx_admin -P "ПАРОЛА" -Q "SELECT 1" -C
```
`1` = паролата е вярна (проблемът е в конфига); `Login failed` = паролата е грешна.

---

## 10. Бонус

**Не деплойвай при промени само в документация.** Добави филтър, за да не хабиш минути/деплой при `.md` промени:

```yaml
on:
  push:
    branches: [ main ]
    paths-ignore:
      - '**.md'
      - 'docs/**'
```

**Ротация на тайни (третирай ги като компрометирани, ако са минали през AI/канали):**
- DB парола: SmarterASP панел → Databases → MS SQL → reset (само букви/цифри).
- RapidAPI ключ: Developer Dashboard → Apps → „Add authorization" (нов ключ) → смени в конфига → изтрий стария.
- SMTP парола: при доставчика (напр. Namecheap PrivateEmail).
След ротация обнови `appsettings.Production.json` на сървъра.

---

## 11. Бърз чеклист за нов проект

- [ ] `appsettings*.json` е в `.gitignore`
- [ ] `appsettings.Production.json` създаден **на сървъра** с реалните тайни
- [ ] `web.config` с `ASPNETCORE_ENVIRONMENT=Production`
- [ ] GitHub Secrets: `FTP_SERVER`, `FTP_USERNAME`, `FTP_PASSWORD`
- [ ] `REMOTE_DIR` установен чрез диагностиката (Стъпка 5)
- [ ] `DOTNET_VERSION` съвпада с `TargetFramework`
- [ ] Първо пускане **без** `--delete` → проверка → после **с** `--delete`
- [ ] (по желание) `paths-ignore` за `**.md`
