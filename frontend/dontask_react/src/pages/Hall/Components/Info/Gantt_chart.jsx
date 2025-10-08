import React, { useState, useMemo } from "react";
import "./Gantt_chart.css";

// --------------------------------------------------------
// КОНСТАНТЫ
// --------------------------------------------------------

const BOARD_COLORS = [
  "#8E8DFF",
  "#6868EE",
  "#EE6868",
  "#EEDB68",
  "#68EE68",
  "#EE68BB",
];

const MONTHS = [
  "Янв",
  "Фев",
  "Мар",
  "Апр",
  "Май",
  "Июн",
  "Июл",
  "Авг",
  "Сен",
  "Окт",
  "Ноя",
  "Дек",
];

// Увеличенная высота строки для доски
const BOARD_BAR_HEIGHT = 60; // Высота самой доски
const ROW_HEIGHT = 75; // Высота строки (BOARD_BAR_HEIGHT + отступ)

// --------------------------------------------------------
// МОКАП ДАННЫХ
// --------------------------------------------------------

const MOCK_BOARDS = [
  {
    boardId: 1,
    boardName: "Дизайн UX/UI",
    dateOfStartWork: "2025-09-01",
    dateOfDeadline: "2025-10-10",
    progressBar: 70,
  },
  {
    boardId: 2,
    boardName: "Бэк-энд разработка",
    dateOfStartWork: "2025-09-01",
    dateOfDeadline: "2025-12-23",
    progressBar: 40,
  },
  {
    boardId: 3,
    boardName: "Фронт-энд разработка",
    dateOfStartWork: "2026-01-01",
    dateOfDeadline: "2026-03-28",
    progressBar: 10,
  },
  {
    boardId: 4,
    boardName: "Интеграция API",
    dateOfStartWork: "2025-11-15",
    dateOfDeadline: "2025-12-31",
    progressBar: 100,
  },
  {
    boardId: 5,
    boardName: "Тестирование",
    dateOfStartWork: "2026-04-01",
    dateOfDeadline: "2026-08-30",
    progressBar: 0,
  },
  {
    boardId: 7,
    boardName: "Дополнительная задача 1",
    dateOfStartWork: "2025-03-01",
    dateOfDeadline: "2025-05-15",
    progressBar: 90,
  },
  {
    boardId: 8,
    boardName: "Дополнительная задача 2",
    dateOfStartWork: "2025-07-20",
    dateOfDeadline: "2025-08-20",
    progressBar: 60,
  },
  {
    boardId: 9,
    boardName: "Дополнительная задача 3",
    dateOfStartWork: "2025-07-20",
    dateOfDeadline: "2025-08-20",
    progressBar: 60,
  },
  {
    boardId: 10,
    boardName: "Дополнительная задача 4",
    dateOfStartWork: "2025-07-20",
    dateOfDeadline: "2025-08-20",
    progressBar: 60,
  },
];

// --------------------------------------------------------
// КОМПОНЕНТ "ДОСКА ГАНТА" (С ИСПРАВЛЕННОЙ ЛОГИКОЙ ТЕКСТА)
// --------------------------------------------------------

const GanttBoard = ({ board, index, selectedYear }) => {
  const { boardName, dateOfStartWork, dateOfDeadline, progressBar } = board;

  if (!dateOfStartWork || !dateOfDeadline) return null;

  // 1. ВЕРТИКАЛЬНОЕ ПОЗИЦИОНИРОВАНИЕ
  const topPosition = index * ROW_HEIGHT + 10;

  // 2. ЦВЕТА
  const color = BOARD_COLORS[index % BOARD_COLORS.length];
  const nonProgressColor = `${color}4D`;

  // 3. РАСЧЕТ ДАТ И ШИРИНЫ (логика без изменений)
  const startDate = new Date(dateOfStartWork);
  const deadlineDate = new Date(dateOfDeadline);

  const boardStartYear = startDate.getFullYear();
  const boardEndYear = deadlineDate.getFullYear();

  const daysInYear = new Date(selectedYear, 1, 29).getMonth() === 1 ? 366 : 365;

  let startDayOfYear;
  let endDayOfYear;

  // Расчет начала (LEFT)
  if (boardStartYear > selectedYear) {
    return null;
  } else if (boardStartYear < selectedYear) {
    startDayOfYear = 0;
  } else {
    const jan1 = new Date(selectedYear, 0, 1);
    startDayOfYear = Math.floor((startDate - jan1) / (1000 * 60 * 60 * 24));
  }

  // Расчет конца (WIDTH)
  if (boardEndYear < selectedYear) {
    return null;
  } else if (boardEndYear > selectedYear) {
    endDayOfYear = daysInYear;
  } else {
    const jan1 = new Date(selectedYear, 0, 1);
    endDayOfYear = Math.floor((deadlineDate - jan1) / (1000 * 60 * 60 * 24));
  }

  const widthInDays = endDayOfYear - startDayOfYear;
  if (widthInDays < 0) return null;

  const left = (startDayOfYear / daysInYear) * 100;
  const width = (widthInDays / daysInYear) * 100;

  // 4. ФОРМАТИРОВАНИЕ
  const formatMonthDay = (date) => {
    return `${String(date.getDate()).padStart(2, "0")}.${String(
      date.getMonth() + 1
    ).padStart(2, "0")}`;
  };

  const datesDisplay = `${formatMonthDay(startDate)} - ${formatMonthDay(
    deadlineDate
  )}`;

  // Если прогресс 0, то ширина прогресс-бара должна быть 0,
  // чтобы min-width не срабатывал.
  const progressWidth = progressBar > 0 ? `${progressBar}%` : `0%`;

  // Определяем, нужно ли отображать светлый текст на фоне
  const showNonProgressText = progressBar < 40;

  return (
    <div
      className="gantt-board-row"
      style={{
        left: `${left}%`,
        width: `${width}%`,
        top: `${topPosition}px`,
        height: `${BOARD_BAR_HEIGHT}px`,
        backgroundColor: nonProgressColor,
      }}
      title={`${boardName} (${datesDisplay})`}
    >
      {/* 1. Контейнер для текста на светлом фоне (если прогресс маленький) */}
      {showNonProgressText && (
        <div className="gantt-non-progress-text-container">
          <p className="gantt-board-name light-text">{boardName}</p>
          <p className="gantt-board-dates light-text">{datesDisplay}</p>
        </div>
      )}

      {/* 2. Прогресс-бар (темный фон) */}
      <div
        className="gantt-progress-bar"
        style={{
          width: progressWidth,
          backgroundColor: color,
        }}
      >
        {/* Текст внутри прогресс-бара (если прогресс достаточно большой) */}
        {progressBar >= 40 && (
          <>
            <p className="gantt-board-name dark-text">{boardName}</p>
            <p className="gantt-board-dates dark-text">{datesDisplay}</p>
          </>
        )}
      </div>
    </div>
  );
};

// --------------------------------------------------------
// ГЛАВНЫЙ КОМПОНЕНТ ДИАГРАММЫ (логика фиксированной высоты)
// --------------------------------------------------------

export default function Gantt_chart() {
  const currentYear = new Date().getFullYear();
  const initialYear = MOCK_BOARDS.some(
    (b) =>
      (b.dateOfStartWork &&
        new Date(b.dateOfStartWork).getFullYear() === currentYear) ||
      (b.dateOfDeadline &&
        new Date(b.dateOfDeadline).getFullYear() === currentYear)
  )
    ? currentYear
    : 2025;

  const [selectedYear, setSelectedYear] = useState(initialYear);

  const availableYears = useMemo(() => {
    const years = new Set();
    MOCK_BOARDS.forEach((b) => {
      if (b.dateOfStartWork)
        years.add(new Date(b.dateOfStartWork).getFullYear());
      if (b.dateOfDeadline) years.add(new Date(b.dateOfDeadline).getFullYear());
    });

    years.add(currentYear);
    years.add(currentYear + 1);

    return Array.from(years)
      .filter((y) => y >= 2000 && y < 2100)
      .sort();
  }, [currentYear]);

  // 1. Определяем все года, которые содержат доски
  const allBoardYears = useMemo(() => {
    const years = new Set();
    MOCK_BOARDS.forEach((b) => {
      if (b.dateOfStartWork)
        years.add(new Date(b.dateOfStartWork).getFullYear());
      if (b.dateOfDeadline) years.add(new Date(b.dateOfDeadline).getFullYear());
    });
    return Array.from(years).sort();
  }, []);

  // 2. Находим максимальное количество досок в любой год (для фиксации высоты)
  const maxRowCount = useMemo(() => {
    let max = 0;

    const yearsToCheck = new Set([
      ...allBoardYears,
      currentYear,
      currentYear + 1,
    ]);

    yearsToCheck.forEach((year) => {
      const count = MOCK_BOARDS.filter((b) => {
        if (!b.dateOfStartWork || !b.dateOfDeadline) return false;
        const startYear = new Date(b.dateOfStartWork).getFullYear();
        const endYear = new Date(b.dateOfDeadline).getFullYear();
        return startYear <= year && endYear >= year;
      }).length;
      max = Math.max(max, count);
    });

    return max;
  }, [allBoardYears, currentYear]);

  // 3. Фиксированная высота внутреннего контента
  const fixedBoardsContentHeight = maxRowCount * ROW_HEIGHT + ROW_HEIGHT;

  const todayPosition = useMemo(() => {
    const today = new Date();
    const currentYearCheck = today.getFullYear();

    if (currentYearCheck !== selectedYear) return -1;

    const jan1 = new Date(selectedYear, 0, 1);
    const dayOfYear = Math.floor((today - jan1) / (1000 * 60 * 60 * 24));
    const daysInYear =
      new Date(selectedYear, 1, 29).getMonth() === 1 ? 366 : 365;

    return (dayOfYear / daysInYear) * 100;
  }, [selectedYear]);

  const currentMonthIndex =
    new Date().getFullYear() === selectedYear ? new Date().getMonth() : -1;

  const renderedBoards = MOCK_BOARDS.filter(
    (b) => b.dateOfStartWork && b.dateOfDeadline
  ).filter((board) => {
    const startYear = new Date(board.dateOfStartWork).getFullYear();
    const endYear = new Date(board.dateOfDeadline).getFullYear();

    return startYear <= selectedYear && endYear >= selectedYear;
  });

  return (
    <div className="gantt-chart-wrapper">
      {/* Выбор года */}
      <div className="gantt-year-selector">
        <select
          value={selectedYear}
          onChange={(e) => setSelectedYear(parseInt(e.target.value))}
        >
          {availableYears.map((year) => (
            <option key={year} value={year}>
              {year}
            </option>
          ))}
        </select>
      </div>

      <div className="gantt-main-container">
        {/* Шапка с месяцами */}
        <div className="gantt-header">
          {MONTHS.map((month, index) => (
            <div
              key={month}
              className={`gantt-month-column ${
                index === currentMonthIndex ? "current-month" : ""
              }`}
            >
              <p className="gantt-month-column-text">{month}</p>
            </div>
          ))}
        </div>

        {/* ✅ Вертикальные линии сетки (Месяцы и Сегодня) */}
        {todayPosition !== -1 && (
          <div
            className="gantt-today-line"
            style={{ left: `${todayPosition}%` }}
          ></div>
        )}

        {/* Вертикальные линии месяцев */}
        {[...Array(12).keys()].map((index) => {
          const monthDayPosition = ((index + 1) / 12) * 100;
          return (
            <div
              key={`vline-${index}`}
              className="gantt-month-line"
              style={{ left: `${monthDayPosition}%` }}
            ></div>
          );
        })}

        {/* Контейнер для досок (прокручиваемый) */}
        <div className="gantt-timeline-container scrollbar-fixed">
          {/* ✅ КОНТЕЙНЕР ДЛЯ ФАКТИЧЕСКОГО КОНТЕНТА, КОТОРЫЙ СКРОЛЛИТСЯ */}
          <div
            className="gantt-boards-content"
            style={{
              // Используем фиксированную максимальную высоту!
              height: `${fixedBoardsContentHeight}px`,
            }}
          >
            {/* Отображение досок */}
            {renderedBoards.map((board, index) => (
              <GanttBoard
                key={board.boardId}
                board={board}
                index={index}
                selectedYear={selectedYear}
              />
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
