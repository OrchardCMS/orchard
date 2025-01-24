/*
** NOTE: This file is generated by Gulp and should not be edited directly!
** Any changes made directly to this file will be overwritten next time its asset group is processed by Gulp.
*/

/* http://keith-wood.name/calendars.html
   Coptic calendar for jQuery v2.0.1.
   Written by Keith Wood (kbwood{at}iinet.com.au) February 2010.
   Available under the MIT (http://keith-wood.name/licence.html) license. 
   Please attribute the author if you use it. */

(function($) { // Hide scope, no $ conflict

	/** Implementation of the Coptic calendar.
		See <a href="http://en.wikipedia.org/wiki/Coptic_calendar">http://en.wikipedia.org/wiki/Coptic_calendar</a>.
		See also Calendrical Calculations: The Millennium Edition
		(<a href="http://emr.cs.iit.edu/home/reingold/calendar-book/index.shtml">http://emr.cs.iit.edu/home/reingold/calendar-book/index.shtml</a>).
		@class CopticCalendar
		@param [language=''] {string} The language code (default English) for localisation. */
	function CopticCalendar(language) {
		this.local = this.regionalOptions[language || ''] || this.regionalOptions[''];
	}

	CopticCalendar.prototype = new $.calendars.baseCalendar;

	$.extend(CopticCalendar.prototype, {
		/** The calendar name.
			@memberof CopticCalendar */
		name: 'Coptic',
		/** Julian date of start of Coptic epoch: 29 August 284 CE (Gregorian).
			@memberof CopticCalendar */
		jdEpoch: 1825029.5,
		/** Days per month in a common year.
			@memberof CopticCalendar */
		daysPerMonth: [30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 5],
		/** <code>true</code> if has a year zero, <code>false</code> if not.
			@memberof CopticCalendar */
		hasYearZero: false,
		/** The minimum month number.
			@memberof CopticCalendar */
		minMonth: 1,
		/** The first month in the year.
			@memberof CopticCalendar */
		firstMonth: 1,
		/** The minimum day number.
			@memberof CopticCalendar */
		minDay: 1,

		/** Localisations for the plugin.
			Entries are objects indexed by the language code ('' being the default US/English).
			Each object has the following attributes.
			@memberof CopticCalendar
			@property name {string} The calendar name.
			@property epochs {string[]} The epoch names.
			@property monthNames {string[]} The long names of the months of the year.
			@property monthNamesShort {string[]} The short names of the months of the year.
			@property dayNames {string[]} The long names of the days of the week.
			@property dayNamesShort {string[]} The short names of the days of the week.
			@property dayNamesMin {string[]} The minimal names of the days of the week.
			@property dateFormat {string} The date format for this calendar.
					See the options on <a href="BaseCalendar.html#formatDate"><code>formatDate</code></a> for details.
			@property firstDay {number} The number of the first day of the week, starting at 0.
			@property isRTL {number} <code>true</code> if this localisation reads right-to-left. */
		regionalOptions: { // Localisations
			'': {
				name: 'Coptic',
				epochs: ['BAM', 'AM'],
				monthNames: ['Thout', 'Paopi', 'Hathor', 'Koiak', 'Tobi', 'Meshir',
				'Paremhat', 'Paremoude', 'Pashons', 'Paoni', 'Epip', 'Mesori', 'Pi Kogi Enavot'],
				monthNamesShort: ['Tho', 'Pao', 'Hath', 'Koi', 'Tob', 'Mesh',
				'Pat', 'Pad', 'Pash', 'Pao', 'Epi', 'Meso', 'PiK'],
				dayNames: ['Tkyriaka', 'Pesnau', 'Pshoment', 'Peftoou', 'Ptiou', 'Psoou', 'Psabbaton'],
				dayNamesShort: ['Tky', 'Pes', 'Psh', 'Pef', 'Pti', 'Pso', 'Psa'],
				dayNamesMin: ['Tk', 'Pes', 'Psh', 'Pef', 'Pt', 'Pso', 'Psa'],
				dateFormat: 'dd/mm/yyyy',
				firstDay: 0,
				isRTL: false
			}
		},

		/** Determine whether this date is in a leap year.
			@memberof CopticCalendar
			@param year {CDate|number} The date to examine or the year to examine.
			@return {boolean} <code>true</code> if this is a leap year, <code>false</code> if not.
			@throws Error if an invalid year or a different calendar used. */
		leapYear: function(year) {
			var date = this._validate(year, this.minMonth, this.minDay, $.calendars.local.invalidYear);
			var year = date.year() + (date.year() < 0 ? 1 : 0); // No year zero
			return year % 4 === 3 || year % 4 === -1;
		},

		/** Retrieve the number of months in a year.
			@memberof CopticCalendar
			@param year {CDate|number} The date to examine or the year to examine.
			@return {number} The number of months.
			@throws Error if an invalid year or a different calendar used. */
		monthsInYear: function(year) {
			this._validate(year, this.minMonth, this.minDay,
				$.calendars.local.invalidYear || $.calendars.regionalOptions[''].invalidYear);
			return 13;
		},

		/** Determine the week of the year for a date.
			@memberof CopticCalendar
			@param year {CDate|number} The date to examine or the year to examine.
			@param [month] {number) the month to examine.
			@param [day] {number} The day to examine.
			@return {number} The week of the year.
			@throws Error if an invalid date or a different calendar used. */
		weekOfYear: function(year, month, day) {
			// Find Sunday of this week starting on Sunday
			var checkDate = this.newDate(year, month, day);
			checkDate.add(-checkDate.dayOfWeek(), 'd');
			return Math.floor((checkDate.dayOfYear() - 1) / 7) + 1;
		},

		/** Retrieve the number of days in a month.
			@memberof CopticCalendar
			@param year {CDate|number} The date to examine or the year of the month.
			@param [month] {number} The month.
			@return {number} The number of days in this month.
			@throws Error if an invalid month/year or a different calendar used. */
		daysInMonth: function(year, month) {
			var date = this._validate(year, month, this.minDay, $.calendars.local.invalidMonth);
			return this.daysPerMonth[date.month() - 1] +
				(date.month() === 13 && this.leapYear(date.year()) ? 1 : 0);
		},

		/** Determine whether this date is a week day.
			@memberof CopticCalendar
			@param year {CDate|number} The date to examine or the year to examine.
			@param month {number} The month to examine.
			@param day {number} The day to examine.
			@return {boolean} <code>true</code> if a week day, <code>false</code> if not.
			@throws Error if an invalid date or a different calendar used. */
		weekDay: function(year, month, day) {
			return (this.dayOfWeek(year, month, day) || 7) < 6;
		},

		/** Retrieve the Julian date equivalent for this date,
			i.e. days since January 1, 4713 BCE Greenwich noon.
			@memberof CopticCalendar
			@param year {CDate|number} The date to convert or the year to convert.
			@param [month] {number) the month to convert.
			@param [day] {number} The day to convert.
			@return {number} The equivalent Julian date.
			@throws Error if an invalid date or a different calendar used. */
		toJD: function(year, month, day) {
			var date = this._validate(year, month, day, $.calendars.local.invalidDate);
			year = date.year();
			if (year < 0) { year++; } // No year zero
			return date.day() + (date.month() - 1) * 30 +
				(year - 1) * 365 + Math.floor(year / 4) + this.jdEpoch - 1;
		},

		/** Create a new date from a Julian date.
			@memberof CopticCalendar
			@param jd {number} The Julian date to convert.
			@return {CDate} The equivalent date. */
		fromJD: function(jd) {
			var c = Math.floor(jd) + 0.5 - this.jdEpoch;
			var year = Math.floor((c - Math.floor((c + 366) / 1461)) / 365) + 1;
			if (year <= 0) { year--; } // No year zero
			c = Math.floor(jd) + 0.5 - this.newDate(year, 1, 1).toJD();
			var month = Math.floor(c / 30) + 1;
			var day = c - (month - 1) * 30 + 1;
			return this.newDate(year, month, day);
		}
	});

	// Coptic calendar implementation
	$.calendars.calendars.coptic = CopticCalendar;

})(jQuery);
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbImpxdWVyeS5jYWxlbmRhcnMuY29wdGljLmpzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiJBQUFBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQSxBQUxBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBO0FBQ0E7QUFDQTtBQUNBIiwiZmlsZSI6ImpxdWVyeS5jYWxlbmRhcnMuY29wdGljLmpzIiwic291cmNlc0NvbnRlbnQiOlsiLyogaHR0cDovL2tlaXRoLXdvb2QubmFtZS9jYWxlbmRhcnMuaHRtbFxyXG4gICBDb3B0aWMgY2FsZW5kYXIgZm9yIGpRdWVyeSB2Mi4wLjEuXHJcbiAgIFdyaXR0ZW4gYnkgS2VpdGggV29vZCAoa2J3b29ke2F0fWlpbmV0LmNvbS5hdSkgRmVicnVhcnkgMjAxMC5cclxuICAgQXZhaWxhYmxlIHVuZGVyIHRoZSBNSVQgKGh0dHA6Ly9rZWl0aC13b29kLm5hbWUvbGljZW5jZS5odG1sKSBsaWNlbnNlLiBcclxuICAgUGxlYXNlIGF0dHJpYnV0ZSB0aGUgYXV0aG9yIGlmIHlvdSB1c2UgaXQuICovXHJcblxyXG4oZnVuY3Rpb24oJCkgeyAvLyBIaWRlIHNjb3BlLCBubyAkIGNvbmZsaWN0XHJcblxyXG5cdC8qKiBJbXBsZW1lbnRhdGlvbiBvZiB0aGUgQ29wdGljIGNhbGVuZGFyLlxyXG5cdFx0U2VlIDxhIGhyZWY9XCJodHRwOi8vZW4ud2lraXBlZGlhLm9yZy93aWtpL0NvcHRpY19jYWxlbmRhclwiPmh0dHA6Ly9lbi53aWtpcGVkaWEub3JnL3dpa2kvQ29wdGljX2NhbGVuZGFyPC9hPi5cclxuXHRcdFNlZSBhbHNvIENhbGVuZHJpY2FsIENhbGN1bGF0aW9uczogVGhlIE1pbGxlbm5pdW0gRWRpdGlvblxyXG5cdFx0KDxhIGhyZWY9XCJodHRwOi8vZW1yLmNzLmlpdC5lZHUvaG9tZS9yZWluZ29sZC9jYWxlbmRhci1ib29rL2luZGV4LnNodG1sXCI+aHR0cDovL2Vtci5jcy5paXQuZWR1L2hvbWUvcmVpbmdvbGQvY2FsZW5kYXItYm9vay9pbmRleC5zaHRtbDwvYT4pLlxyXG5cdFx0QGNsYXNzIENvcHRpY0NhbGVuZGFyXHJcblx0XHRAcGFyYW0gW2xhbmd1YWdlPScnXSB7c3RyaW5nfSBUaGUgbGFuZ3VhZ2UgY29kZSAoZGVmYXVsdCBFbmdsaXNoKSBmb3IgbG9jYWxpc2F0aW9uLiAqL1xyXG5cdGZ1bmN0aW9uIENvcHRpY0NhbGVuZGFyKGxhbmd1YWdlKSB7XHJcblx0XHR0aGlzLmxvY2FsID0gdGhpcy5yZWdpb25hbE9wdGlvbnNbbGFuZ3VhZ2UgfHwgJyddIHx8IHRoaXMucmVnaW9uYWxPcHRpb25zWycnXTtcclxuXHR9XHJcblxyXG5cdENvcHRpY0NhbGVuZGFyLnByb3RvdHlwZSA9IG5ldyAkLmNhbGVuZGFycy5iYXNlQ2FsZW5kYXI7XHJcblxyXG5cdCQuZXh0ZW5kKENvcHRpY0NhbGVuZGFyLnByb3RvdHlwZSwge1xyXG5cdFx0LyoqIFRoZSBjYWxlbmRhciBuYW1lLlxyXG5cdFx0XHRAbWVtYmVyb2YgQ29wdGljQ2FsZW5kYXIgKi9cclxuXHRcdG5hbWU6ICdDb3B0aWMnLFxyXG5cdFx0LyoqIEp1bGlhbiBkYXRlIG9mIHN0YXJ0IG9mIENvcHRpYyBlcG9jaDogMjkgQXVndXN0IDI4NCBDRSAoR3JlZ29yaWFuKS5cclxuXHRcdFx0QG1lbWJlcm9mIENvcHRpY0NhbGVuZGFyICovXHJcblx0XHRqZEVwb2NoOiAxODI1MDI5LjUsXHJcblx0XHQvKiogRGF5cyBwZXIgbW9udGggaW4gYSBjb21tb24geWVhci5cclxuXHRcdFx0QG1lbWJlcm9mIENvcHRpY0NhbGVuZGFyICovXHJcblx0XHRkYXlzUGVyTW9udGg6IFszMCwgMzAsIDMwLCAzMCwgMzAsIDMwLCAzMCwgMzAsIDMwLCAzMCwgMzAsIDMwLCA1XSxcclxuXHRcdC8qKiA8Y29kZT50cnVlPC9jb2RlPiBpZiBoYXMgYSB5ZWFyIHplcm8sIDxjb2RlPmZhbHNlPC9jb2RlPiBpZiBub3QuXHJcblx0XHRcdEBtZW1iZXJvZiBDb3B0aWNDYWxlbmRhciAqL1xyXG5cdFx0aGFzWWVhclplcm86IGZhbHNlLFxyXG5cdFx0LyoqIFRoZSBtaW5pbXVtIG1vbnRoIG51bWJlci5cclxuXHRcdFx0QG1lbWJlcm9mIENvcHRpY0NhbGVuZGFyICovXHJcblx0XHRtaW5Nb250aDogMSxcclxuXHRcdC8qKiBUaGUgZmlyc3QgbW9udGggaW4gdGhlIHllYXIuXHJcblx0XHRcdEBtZW1iZXJvZiBDb3B0aWNDYWxlbmRhciAqL1xyXG5cdFx0Zmlyc3RNb250aDogMSxcclxuXHRcdC8qKiBUaGUgbWluaW11bSBkYXkgbnVtYmVyLlxyXG5cdFx0XHRAbWVtYmVyb2YgQ29wdGljQ2FsZW5kYXIgKi9cclxuXHRcdG1pbkRheTogMSxcclxuXHJcblx0XHQvKiogTG9jYWxpc2F0aW9ucyBmb3IgdGhlIHBsdWdpbi5cclxuXHRcdFx0RW50cmllcyBhcmUgb2JqZWN0cyBpbmRleGVkIGJ5IHRoZSBsYW5ndWFnZSBjb2RlICgnJyBiZWluZyB0aGUgZGVmYXVsdCBVUy9FbmdsaXNoKS5cclxuXHRcdFx0RWFjaCBvYmplY3QgaGFzIHRoZSBmb2xsb3dpbmcgYXR0cmlidXRlcy5cclxuXHRcdFx0QG1lbWJlcm9mIENvcHRpY0NhbGVuZGFyXHJcblx0XHRcdEBwcm9wZXJ0eSBuYW1lIHtzdHJpbmd9IFRoZSBjYWxlbmRhciBuYW1lLlxyXG5cdFx0XHRAcHJvcGVydHkgZXBvY2hzIHtzdHJpbmdbXX0gVGhlIGVwb2NoIG5hbWVzLlxyXG5cdFx0XHRAcHJvcGVydHkgbW9udGhOYW1lcyB7c3RyaW5nW119IFRoZSBsb25nIG5hbWVzIG9mIHRoZSBtb250aHMgb2YgdGhlIHllYXIuXHJcblx0XHRcdEBwcm9wZXJ0eSBtb250aE5hbWVzU2hvcnQge3N0cmluZ1tdfSBUaGUgc2hvcnQgbmFtZXMgb2YgdGhlIG1vbnRocyBvZiB0aGUgeWVhci5cclxuXHRcdFx0QHByb3BlcnR5IGRheU5hbWVzIHtzdHJpbmdbXX0gVGhlIGxvbmcgbmFtZXMgb2YgdGhlIGRheXMgb2YgdGhlIHdlZWsuXHJcblx0XHRcdEBwcm9wZXJ0eSBkYXlOYW1lc1Nob3J0IHtzdHJpbmdbXX0gVGhlIHNob3J0IG5hbWVzIG9mIHRoZSBkYXlzIG9mIHRoZSB3ZWVrLlxyXG5cdFx0XHRAcHJvcGVydHkgZGF5TmFtZXNNaW4ge3N0cmluZ1tdfSBUaGUgbWluaW1hbCBuYW1lcyBvZiB0aGUgZGF5cyBvZiB0aGUgd2Vlay5cclxuXHRcdFx0QHByb3BlcnR5IGRhdGVGb3JtYXQge3N0cmluZ30gVGhlIGRhdGUgZm9ybWF0IGZvciB0aGlzIGNhbGVuZGFyLlxyXG5cdFx0XHRcdFx0U2VlIHRoZSBvcHRpb25zIG9uIDxhIGhyZWY9XCJCYXNlQ2FsZW5kYXIuaHRtbCNmb3JtYXREYXRlXCI+PGNvZGU+Zm9ybWF0RGF0ZTwvY29kZT48L2E+IGZvciBkZXRhaWxzLlxyXG5cdFx0XHRAcHJvcGVydHkgZmlyc3REYXkge251bWJlcn0gVGhlIG51bWJlciBvZiB0aGUgZmlyc3QgZGF5IG9mIHRoZSB3ZWVrLCBzdGFydGluZyBhdCAwLlxyXG5cdFx0XHRAcHJvcGVydHkgaXNSVEwge251bWJlcn0gPGNvZGU+dHJ1ZTwvY29kZT4gaWYgdGhpcyBsb2NhbGlzYXRpb24gcmVhZHMgcmlnaHQtdG8tbGVmdC4gKi9cclxuXHRcdHJlZ2lvbmFsT3B0aW9uczogeyAvLyBMb2NhbGlzYXRpb25zXHJcblx0XHRcdCcnOiB7XHJcblx0XHRcdFx0bmFtZTogJ0NvcHRpYycsXHJcblx0XHRcdFx0ZXBvY2hzOiBbJ0JBTScsICdBTSddLFxyXG5cdFx0XHRcdG1vbnRoTmFtZXM6IFsnVGhvdXQnLCAnUGFvcGknLCAnSGF0aG9yJywgJ0tvaWFrJywgJ1RvYmknLCAnTWVzaGlyJyxcclxuXHRcdFx0XHQnUGFyZW1oYXQnLCAnUGFyZW1vdWRlJywgJ1Bhc2hvbnMnLCAnUGFvbmknLCAnRXBpcCcsICdNZXNvcmknLCAnUGkgS29naSBFbmF2b3QnXSxcclxuXHRcdFx0XHRtb250aE5hbWVzU2hvcnQ6IFsnVGhvJywgJ1BhbycsICdIYXRoJywgJ0tvaScsICdUb2InLCAnTWVzaCcsXHJcblx0XHRcdFx0J1BhdCcsICdQYWQnLCAnUGFzaCcsICdQYW8nLCAnRXBpJywgJ01lc28nLCAnUGlLJ10sXHJcblx0XHRcdFx0ZGF5TmFtZXM6IFsnVGt5cmlha2EnLCAnUGVzbmF1JywgJ1BzaG9tZW50JywgJ1BlZnRvb3UnLCAnUHRpb3UnLCAnUHNvb3UnLCAnUHNhYmJhdG9uJ10sXHJcblx0XHRcdFx0ZGF5TmFtZXNTaG9ydDogWydUa3knLCAnUGVzJywgJ1BzaCcsICdQZWYnLCAnUHRpJywgJ1BzbycsICdQc2EnXSxcclxuXHRcdFx0XHRkYXlOYW1lc01pbjogWydUaycsICdQZXMnLCAnUHNoJywgJ1BlZicsICdQdCcsICdQc28nLCAnUHNhJ10sXHJcblx0XHRcdFx0ZGF0ZUZvcm1hdDogJ2RkL21tL3l5eXknLFxyXG5cdFx0XHRcdGZpcnN0RGF5OiAwLFxyXG5cdFx0XHRcdGlzUlRMOiBmYWxzZVxyXG5cdFx0XHR9XHJcblx0XHR9LFxyXG5cclxuXHRcdC8qKiBEZXRlcm1pbmUgd2hldGhlciB0aGlzIGRhdGUgaXMgaW4gYSBsZWFwIHllYXIuXHJcblx0XHRcdEBtZW1iZXJvZiBDb3B0aWNDYWxlbmRhclxyXG5cdFx0XHRAcGFyYW0geWVhciB7Q0RhdGV8bnVtYmVyfSBUaGUgZGF0ZSB0byBleGFtaW5lIG9yIHRoZSB5ZWFyIHRvIGV4YW1pbmUuXHJcblx0XHRcdEByZXR1cm4ge2Jvb2xlYW59IDxjb2RlPnRydWU8L2NvZGU+IGlmIHRoaXMgaXMgYSBsZWFwIHllYXIsIDxjb2RlPmZhbHNlPC9jb2RlPiBpZiBub3QuXHJcblx0XHRcdEB0aHJvd3MgRXJyb3IgaWYgYW4gaW52YWxpZCB5ZWFyIG9yIGEgZGlmZmVyZW50IGNhbGVuZGFyIHVzZWQuICovXHJcblx0XHRsZWFwWWVhcjogZnVuY3Rpb24oeWVhcikge1xyXG5cdFx0XHR2YXIgZGF0ZSA9IHRoaXMuX3ZhbGlkYXRlKHllYXIsIHRoaXMubWluTW9udGgsIHRoaXMubWluRGF5LCAkLmNhbGVuZGFycy5sb2NhbC5pbnZhbGlkWWVhcik7XHJcblx0XHRcdHZhciB5ZWFyID0gZGF0ZS55ZWFyKCkgKyAoZGF0ZS55ZWFyKCkgPCAwID8gMSA6IDApOyAvLyBObyB5ZWFyIHplcm9cclxuXHRcdFx0cmV0dXJuIHllYXIgJSA0ID09PSAzIHx8IHllYXIgJSA0ID09PSAtMTtcclxuXHRcdH0sXHJcblxyXG5cdFx0LyoqIFJldHJpZXZlIHRoZSBudW1iZXIgb2YgbW9udGhzIGluIGEgeWVhci5cclxuXHRcdFx0QG1lbWJlcm9mIENvcHRpY0NhbGVuZGFyXHJcblx0XHRcdEBwYXJhbSB5ZWFyIHtDRGF0ZXxudW1iZXJ9IFRoZSBkYXRlIHRvIGV4YW1pbmUgb3IgdGhlIHllYXIgdG8gZXhhbWluZS5cclxuXHRcdFx0QHJldHVybiB7bnVtYmVyfSBUaGUgbnVtYmVyIG9mIG1vbnRocy5cclxuXHRcdFx0QHRocm93cyBFcnJvciBpZiBhbiBpbnZhbGlkIHllYXIgb3IgYSBkaWZmZXJlbnQgY2FsZW5kYXIgdXNlZC4gKi9cclxuXHRcdG1vbnRoc0luWWVhcjogZnVuY3Rpb24oeWVhcikge1xyXG5cdFx0XHR0aGlzLl92YWxpZGF0ZSh5ZWFyLCB0aGlzLm1pbk1vbnRoLCB0aGlzLm1pbkRheSxcclxuXHRcdFx0XHQkLmNhbGVuZGFycy5sb2NhbC5pbnZhbGlkWWVhciB8fCAkLmNhbGVuZGFycy5yZWdpb25hbE9wdGlvbnNbJyddLmludmFsaWRZZWFyKTtcclxuXHRcdFx0cmV0dXJuIDEzO1xyXG5cdFx0fSxcclxuXHJcblx0XHQvKiogRGV0ZXJtaW5lIHRoZSB3ZWVrIG9mIHRoZSB5ZWFyIGZvciBhIGRhdGUuXHJcblx0XHRcdEBtZW1iZXJvZiBDb3B0aWNDYWxlbmRhclxyXG5cdFx0XHRAcGFyYW0geWVhciB7Q0RhdGV8bnVtYmVyfSBUaGUgZGF0ZSB0byBleGFtaW5lIG9yIHRoZSB5ZWFyIHRvIGV4YW1pbmUuXHJcblx0XHRcdEBwYXJhbSBbbW9udGhdIHtudW1iZXIpIHRoZSBtb250aCB0byBleGFtaW5lLlxyXG5cdFx0XHRAcGFyYW0gW2RheV0ge251bWJlcn0gVGhlIGRheSB0byBleGFtaW5lLlxyXG5cdFx0XHRAcmV0dXJuIHtudW1iZXJ9IFRoZSB3ZWVrIG9mIHRoZSB5ZWFyLlxyXG5cdFx0XHRAdGhyb3dzIEVycm9yIGlmIGFuIGludmFsaWQgZGF0ZSBvciBhIGRpZmZlcmVudCBjYWxlbmRhciB1c2VkLiAqL1xyXG5cdFx0d2Vla09mWWVhcjogZnVuY3Rpb24oeWVhciwgbW9udGgsIGRheSkge1xyXG5cdFx0XHQvLyBGaW5kIFN1bmRheSBvZiB0aGlzIHdlZWsgc3RhcnRpbmcgb24gU3VuZGF5XHJcblx0XHRcdHZhciBjaGVja0RhdGUgPSB0aGlzLm5ld0RhdGUoeWVhciwgbW9udGgsIGRheSk7XHJcblx0XHRcdGNoZWNrRGF0ZS5hZGQoLWNoZWNrRGF0ZS5kYXlPZldlZWsoKSwgJ2QnKTtcclxuXHRcdFx0cmV0dXJuIE1hdGguZmxvb3IoKGNoZWNrRGF0ZS5kYXlPZlllYXIoKSAtIDEpIC8gNykgKyAxO1xyXG5cdFx0fSxcclxuXHJcblx0XHQvKiogUmV0cmlldmUgdGhlIG51bWJlciBvZiBkYXlzIGluIGEgbW9udGguXHJcblx0XHRcdEBtZW1iZXJvZiBDb3B0aWNDYWxlbmRhclxyXG5cdFx0XHRAcGFyYW0geWVhciB7Q0RhdGV8bnVtYmVyfSBUaGUgZGF0ZSB0byBleGFtaW5lIG9yIHRoZSB5ZWFyIG9mIHRoZSBtb250aC5cclxuXHRcdFx0QHBhcmFtIFttb250aF0ge251bWJlcn0gVGhlIG1vbnRoLlxyXG5cdFx0XHRAcmV0dXJuIHtudW1iZXJ9IFRoZSBudW1iZXIgb2YgZGF5cyBpbiB0aGlzIG1vbnRoLlxyXG5cdFx0XHRAdGhyb3dzIEVycm9yIGlmIGFuIGludmFsaWQgbW9udGgveWVhciBvciBhIGRpZmZlcmVudCBjYWxlbmRhciB1c2VkLiAqL1xyXG5cdFx0ZGF5c0luTW9udGg6IGZ1bmN0aW9uKHllYXIsIG1vbnRoKSB7XHJcblx0XHRcdHZhciBkYXRlID0gdGhpcy5fdmFsaWRhdGUoeWVhciwgbW9udGgsIHRoaXMubWluRGF5LCAkLmNhbGVuZGFycy5sb2NhbC5pbnZhbGlkTW9udGgpO1xyXG5cdFx0XHRyZXR1cm4gdGhpcy5kYXlzUGVyTW9udGhbZGF0ZS5tb250aCgpIC0gMV0gK1xyXG5cdFx0XHRcdChkYXRlLm1vbnRoKCkgPT09IDEzICYmIHRoaXMubGVhcFllYXIoZGF0ZS55ZWFyKCkpID8gMSA6IDApO1xyXG5cdFx0fSxcclxuXHJcblx0XHQvKiogRGV0ZXJtaW5lIHdoZXRoZXIgdGhpcyBkYXRlIGlzIGEgd2VlayBkYXkuXHJcblx0XHRcdEBtZW1iZXJvZiBDb3B0aWNDYWxlbmRhclxyXG5cdFx0XHRAcGFyYW0geWVhciB7Q0RhdGV8bnVtYmVyfSBUaGUgZGF0ZSB0byBleGFtaW5lIG9yIHRoZSB5ZWFyIHRvIGV4YW1pbmUuXHJcblx0XHRcdEBwYXJhbSBtb250aCB7bnVtYmVyfSBUaGUgbW9udGggdG8gZXhhbWluZS5cclxuXHRcdFx0QHBhcmFtIGRheSB7bnVtYmVyfSBUaGUgZGF5IHRvIGV4YW1pbmUuXHJcblx0XHRcdEByZXR1cm4ge2Jvb2xlYW59IDxjb2RlPnRydWU8L2NvZGU+IGlmIGEgd2VlayBkYXksIDxjb2RlPmZhbHNlPC9jb2RlPiBpZiBub3QuXHJcblx0XHRcdEB0aHJvd3MgRXJyb3IgaWYgYW4gaW52YWxpZCBkYXRlIG9yIGEgZGlmZmVyZW50IGNhbGVuZGFyIHVzZWQuICovXHJcblx0XHR3ZWVrRGF5OiBmdW5jdGlvbih5ZWFyLCBtb250aCwgZGF5KSB7XHJcblx0XHRcdHJldHVybiAodGhpcy5kYXlPZldlZWsoeWVhciwgbW9udGgsIGRheSkgfHwgNykgPCA2O1xyXG5cdFx0fSxcclxuXHJcblx0XHQvKiogUmV0cmlldmUgdGhlIEp1bGlhbiBkYXRlIGVxdWl2YWxlbnQgZm9yIHRoaXMgZGF0ZSxcclxuXHRcdFx0aS5lLiBkYXlzIHNpbmNlIEphbnVhcnkgMSwgNDcxMyBCQ0UgR3JlZW53aWNoIG5vb24uXHJcblx0XHRcdEBtZW1iZXJvZiBDb3B0aWNDYWxlbmRhclxyXG5cdFx0XHRAcGFyYW0geWVhciB7Q0RhdGV8bnVtYmVyfSBUaGUgZGF0ZSB0byBjb252ZXJ0IG9yIHRoZSB5ZWFyIHRvIGNvbnZlcnQuXHJcblx0XHRcdEBwYXJhbSBbbW9udGhdIHtudW1iZXIpIHRoZSBtb250aCB0byBjb252ZXJ0LlxyXG5cdFx0XHRAcGFyYW0gW2RheV0ge251bWJlcn0gVGhlIGRheSB0byBjb252ZXJ0LlxyXG5cdFx0XHRAcmV0dXJuIHtudW1iZXJ9IFRoZSBlcXVpdmFsZW50IEp1bGlhbiBkYXRlLlxyXG5cdFx0XHRAdGhyb3dzIEVycm9yIGlmIGFuIGludmFsaWQgZGF0ZSBvciBhIGRpZmZlcmVudCBjYWxlbmRhciB1c2VkLiAqL1xyXG5cdFx0dG9KRDogZnVuY3Rpb24oeWVhciwgbW9udGgsIGRheSkge1xyXG5cdFx0XHR2YXIgZGF0ZSA9IHRoaXMuX3ZhbGlkYXRlKHllYXIsIG1vbnRoLCBkYXksICQuY2FsZW5kYXJzLmxvY2FsLmludmFsaWREYXRlKTtcclxuXHRcdFx0eWVhciA9IGRhdGUueWVhcigpO1xyXG5cdFx0XHRpZiAoeWVhciA8IDApIHsgeWVhcisrOyB9IC8vIE5vIHllYXIgemVyb1xyXG5cdFx0XHRyZXR1cm4gZGF0ZS5kYXkoKSArIChkYXRlLm1vbnRoKCkgLSAxKSAqIDMwICtcclxuXHRcdFx0XHQoeWVhciAtIDEpICogMzY1ICsgTWF0aC5mbG9vcih5ZWFyIC8gNCkgKyB0aGlzLmpkRXBvY2ggLSAxO1xyXG5cdFx0fSxcclxuXHJcblx0XHQvKiogQ3JlYXRlIGEgbmV3IGRhdGUgZnJvbSBhIEp1bGlhbiBkYXRlLlxyXG5cdFx0XHRAbWVtYmVyb2YgQ29wdGljQ2FsZW5kYXJcclxuXHRcdFx0QHBhcmFtIGpkIHtudW1iZXJ9IFRoZSBKdWxpYW4gZGF0ZSB0byBjb252ZXJ0LlxyXG5cdFx0XHRAcmV0dXJuIHtDRGF0ZX0gVGhlIGVxdWl2YWxlbnQgZGF0ZS4gKi9cclxuXHRcdGZyb21KRDogZnVuY3Rpb24oamQpIHtcclxuXHRcdFx0dmFyIGMgPSBNYXRoLmZsb29yKGpkKSArIDAuNSAtIHRoaXMuamRFcG9jaDtcclxuXHRcdFx0dmFyIHllYXIgPSBNYXRoLmZsb29yKChjIC0gTWF0aC5mbG9vcigoYyArIDM2NikgLyAxNDYxKSkgLyAzNjUpICsgMTtcclxuXHRcdFx0aWYgKHllYXIgPD0gMCkgeyB5ZWFyLS07IH0gLy8gTm8geWVhciB6ZXJvXHJcblx0XHRcdGMgPSBNYXRoLmZsb29yKGpkKSArIDAuNSAtIHRoaXMubmV3RGF0ZSh5ZWFyLCAxLCAxKS50b0pEKCk7XHJcblx0XHRcdHZhciBtb250aCA9IE1hdGguZmxvb3IoYyAvIDMwKSArIDE7XHJcblx0XHRcdHZhciBkYXkgPSBjIC0gKG1vbnRoIC0gMSkgKiAzMCArIDE7XHJcblx0XHRcdHJldHVybiB0aGlzLm5ld0RhdGUoeWVhciwgbW9udGgsIGRheSk7XHJcblx0XHR9XHJcblx0fSk7XHJcblxyXG5cdC8vIENvcHRpYyBjYWxlbmRhciBpbXBsZW1lbnRhdGlvblxyXG5cdCQuY2FsZW5kYXJzLmNhbGVuZGFycy5jb3B0aWMgPSBDb3B0aWNDYWxlbmRhcjtcclxuXHJcbn0pKGpRdWVyeSk7Il0sInNvdXJjZVJvb3QiOiIvc291cmNlLyJ9
