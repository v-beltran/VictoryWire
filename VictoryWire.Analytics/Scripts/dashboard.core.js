(function (win, doc) {

    var VW = VW || {};
    VW.Dashboard = VW.Dashboard || {};

    if (!win.VW) {
        win.VW = VW;
    };

})(window, document);

VW.Dashboard = (function () {

    return {

        VWBaseUrl: "", // no trailing slash

        APIBaseUrl: "", // no trailing slash

        CurrentView: "payroll",

        Init: function () {
            /* Payroll view is loaded first */
            this.ToggleView(this.CurrentView);

            /* Handle click events */
            $(document).on("click", ".db-nav-link", function () {
                // Toggle current page indicator
                $(".db-current-link").removeClass("db-current-link");
                $(this).addClass("db-current-link");

                // Load selected view
                var view = $(this).attr("data-view");
                VW.Dashboard.ToggleView(view);
            }).on("click", ".db-action-link", function () {
                var action = $(this).attr("data-action");

                // Handle action
                switch (action) {
                    case "refresh":
                        VW.Dashboard.RefreshView();             
                        break;
                    case "download":
                        VW.Dashboard.DownloadView();                  
                        break;
                }
            }).on("change", ".panel-filter select", function () {
                var filter = $(this).val();
                var panelId = $(this).parents(".panel").first().attr("id");

                // Load selected filter for report
                VW.Dashboard.FilterView(panelId, filter);
            });

            /* Auto-refresh current view every 15 minutes */
            setTimeout(function () { VW.Dashboard.RefreshView(); }, 900000);
        },

        DownloadView: function () {
            // Download *something* about the current view
            switch (this.CurrentView) {
                case "payroll":
                    this.Payroll.Download();
                    break;
            }         

        },

        FilterView: function (panelId, filter) {
            var viewId = "";
            var filterAll = (panelId == "" && filter == "");

            // Filter specific report
            switch (this.CurrentView) {
                case "payroll":
                    viewId = "#db-payroll";
                    if (!filterAll) { this.Payroll.Filter(panelId, filter); }
                    break;
            }

            // Filter entire view
            if (filterAll) {
                $(viewId + " .panel-filter," + viewId + " .panel-empty-filter").each(function () {
                    if ($(this).is(":visible")) {
                        var select = $(this).find("select");
                        var currentFilter = (select.length > 0) ? select.find("option:selected").val() : "";
                        var currentReport = (select.length > 0) ? select.parents(".panel").first().attr("id") : $(this).parents(".panel").first().attr("id");

                        switch (VW.Dashboard.CurrentView) {
                            case "payroll":
                                VW.Dashboard.Payroll.Filter(currentReport, currentFilter);
                                break;
                        }
                    }
                });
            }
        },

        RefreshView: function () {
            // Reload current view
            switch (this.CurrentView) {
                case "payroll":
                    this.Payroll.Loaded = false;
                    break;
            }
            VW.Dashboard.Output.FixStuff();
            this.ToggleView(this.CurrentView);
        },

        ToggleView: function (view) {
            // Hide all views by default
            $(".db-view").hide();

            /* Show selected view */
            switch (view) {
                case "payroll":
                    $("#db-payroll").show();
                    this.Payroll.Init();
                    break;
            }

            VW.Dashboard.Output.FixStuff();

            // Set current view
            this.CurrentView = view;

            // Load filters for selected view
            this.FilterView("", "");
        },

        ToggleAjaxSpinner: function (panelId, show) {
            if (show) {
                $(panelId).addClass("panel-loading");
            } else {
                $(panelId).removeClass("panel-loading");
            }
        },

        SetDownloadUrl: function (type, name, filter, panelId) {
            var link = $(panelId).find(".panel-download a");
            var url = this.APIBaseUrl + "/report/download/?type=" + type + "&name=" + name + "&filter=" + filter;

            if (link.length > 0) {
                link.attr("href", url);
            }
        },

        LoadData: function (type, name, filter, panelId, successCallback) {
            $.ajax({
                method: "GET",
                url: this.APIBaseUrl + "/report/data/",
                contentType: "application/javascript",
                dataType: "jsonp",
                data: { "type": type, "name": name, "filter": filter },
                beforeSend: function () { VW.Dashboard.Output.Write(panelId + " .panel-body", ""); VW.Dashboard.ToggleAjaxSpinner(panelId, true); }
            }).done(successCallback).fail(function (jqXHR, textStatus) {
                console.log("ERROR: {\"type\": \"" + type + "\", \"name\": \"" + name + "\", \"filter\": \"" + filter + "\"})");
            }).always(function () {
                VW.Dashboard.ToggleAjaxSpinner(panelId, false);

            });
        }

    

    }

})();

VW.Dashboard.Payroll = (function () {

    return {

        Loaded: false,

        Init: function () {

            if (!this.Loaded) {           

                /* Total Accounts */
                VW.Dashboard.LoadData("payroll", "totalaccounts", "", "#db-payroll-totalaccounts", function (data) {
                    if (typeof data.text !== "undefined") {
                        VW.Dashboard.Output.Text("#db-payroll-totalaccounts .panel-body", data.text);
                    }
                });

                /* Newest Accounts */
                VW.Dashboard.SetDownloadUrl("payroll", "newaccounts", "", "#db-payroll-newaccounts");
                VW.Dashboard.LoadData("payroll", "newaccounts", "", "#db-payroll-newaccounts", function (data) {
                    if (typeof data.list !== "undefined") {
                        for (var i = 0; i < data.list.length; i++) {
                            var joinDate = VW.Dashboard.Output.CleanDateTime(data.list[i].joindate);
                            var html = "<div>";
                            html += "<div>" + data.list[i].name + " @ " + data.list[i].company + "</div>";
                            html += "<div>Joined: " + joinDate.toShortFormattedString() + "</div>";
                            html += "</div>";
                            html += "<div></div>";
                            data.list[i] = html;
                        }
                        VW.Dashboard.Output.List("#db-payroll-newaccounts .panel-body", data.list, "string");
                    }
                });

                /* Largest Industries */
                VW.Dashboard.LoadData("payroll", "largestindustries", "", "#db-payroll-largestindustries", function (data) {
                    if (typeof data.list !== "undefined") {
                        for (var i = 0; i < data.list.length; i++) {
                            var html = "<div>";
                            html += "<div>" + data.list[i].industry + "</div>";
                            html += "</div>";
                            html += "<div>" + data.list[i].count + "</div>";
                            data.list[i] = html;
                        }
                        VW.Dashboard.Output.List("#db-payroll-largestindustries .panel-body", data.list, "number");
                    }
                });
                
                /* Total Money Paid */
                VW.Dashboard.LoadData("payroll", "totalmoney", "", "#db-payroll-totalmoney", function (data) {
                    if (typeof data.text !== "undefined") {
                        VW.Dashboard.Output.Text("#db-payroll-totalmoney .panel-body", data.text);
                    }
                });

                /* Most Populous States */
                VW.Dashboard.LoadData("payroll", "mostpopulous", "", "#db-payroll-mostpopulous", function (data) {
                    if (typeof data.list !== "undefined") {
                        for (var i = 0; i < data.list.length; i++) {
                            var html = "<div>";
                            html += "<div>" + data.list[i].state + "</div>";
                            html += "</div>";
                            html += "<div>" + data.list[i].count + "</div>";
                            data.list[i] = html;
                        }
                        VW.Dashboard.Output.List("#db-payroll-mostpopulous .panel-body", data.list, "number");
                    }
                });

                /* Newest Employee Hires */
                VW.Dashboard.SetDownloadUrl("payroll", "newhires", "", "#db-payroll-newhires");
                VW.Dashboard.LoadData("payroll", "newhires", "", "#db-payroll-newhires", function (data) {
                    if (typeof data.list !== "undefined") {
                        for (var i = 0; i < data.list.length; i++) {
                            var joinDate = VW.Dashboard.Output.CleanDateTime(data.list[i].hiredate);
                            var html = "<div>";
                            html += "<div>" + data.list[i].name + " @ " + data.list[i].company + "</div>";
                            html += "<div>Joined: " + joinDate.toShortFormattedString() + "</div>";
                            html += "</div>";
                            html += "<div></div>";
                            data.list[i] = html;
                        }
                        VW.Dashboard.Output.List("#db-payroll-newhires .panel-body", data.list, "string");
                    }
                });

                /* Total Employee Terminations */
                VW.Dashboard.LoadData("payroll", "totalterminations", "", "#db-payroll-totalterminations", function (data) {
                    if (typeof data.text !== "undefined") {
                        VW.Dashboard.Output.Text("#db-payroll-totalterminations .panel-body", data.text);
                    }
                });               

                // Set loaded flag
                this.Loaded = true;
            }
        },

        Filter: function (panelId, filter) {
            var report = panelId.replace(/db-payroll-/g, "");

            switch (report) {

                /* Accounts Created Over Time */
                case "accountscreated":
                    VW.Dashboard.LoadData("payroll", report, filter, "#" + panelId, function (data) {
                        if (typeof data.chart !== "undefined" && data.chart.length > 0) {
                            var keys = {
                                //x: 'date',
                                value: ['count']
                            };

                            var axis = {
                                x: {
                                    //type: 'category',
                                    padding: {
                                        left: 0,
                                        right: 0
                                    },
                                    tick: {
                                        format: function (x) { return data.chart[x].date; }
                                    }
                                },
                                y: {
                                    tick: {
                                        count: 6,
                                        format: function (x) { return Math.ceil(x); }
                                    }
                                }
                            };

                            VW.Dashboard.Output.Chart("#" + panelId + " .panel-body", "area", data.chart, keys, axis);
                        }
                    });
                    break;

                /* Highest-Paid Employees By Week */
                case "highestpaid":
                    VW.Dashboard.LoadData("payroll", report, filter, "#" + panelId, function (data) {
                        if (typeof data.chart !== "undefined" && data.chart.length > 0) {
                            var keys = {
                                x: 'name',
                                value: ['count']
                            };

                            var axis = {
                                x: {
                                    type: 'category',
                                    padding: {
                                        left: 0,
                                        right: 0
                                    }
                                },
                                y: {
                                    tick: {
                                        count: 8,
                                        format: d3.format("$.2f,")
                                    }
                                }
                            };

                            var padding = { top: 20, right: 20, bottom: 20, left: 75 };

                            VW.Dashboard.Output.Chart("#" + panelId + " .panel-body", "bar", data.chart, keys, axis, padding);
                        }
                    });
                    break;
            }

        },

        Download: function () {

        }
    }

})();

VW.Dashboard.Output = (function () {

    return {

        Write: function (target, data) {
            $(target).html(data);
        },

        Chart: function (target, chartType, valuesJSON, keysJSON, axisJSON, paddingJSON) {
            var colors = ['#42BAC7', '#676C7D', '#018DC8', '#39C051', '#E0E6ED'];
            return c3.generate({
                bindto: target,
                data: {
                    type: chartType,
                    json: valuesJSON,
                    keys: keysJSON,
                    color: function (color, d) {
                        return chartType == "bar" ? colors[d.index] : color;
                    }
                },
                axis: axisJSON,
                grid: {
                    y: {
                        show: true
                    }
                },
                legend: {
                    show: false
                },
                padding: (typeof paddingJSON !== "undefined") ? paddingJSON : { top: 20, right: 20, bottom: 20, left: 50 },
                point: {
                    r: 7,
                },
                color: {
                    pattern: colors
                },
                tooltip: {
                    contents: function (d, defaultTitleFormat, defaultValueFormat, color) {
                        color = function () { return chartType == "bar" ? colors[d[0].index] : colors[0]; };
                        return this.getTooltipContent(d, defaultTitleFormat, defaultValueFormat, color)
                    }
                }
            });
        },

        Text: function (target, data) {
            var output = "<div class='panel-text'><div>" + data + "</div></div>";
            this.Write(target, output);

            VW.Dashboard.Output.FixStuff();


        },

        FixStuff: function () {

            $(".panel-text").each(function () {

                //set it to the default
                $(this).css("font-size", "139px");
     
                    while( $(this).find("div").width() >=  ($(this).width() -10) )
                    {
                        //adjust font size until it fits 
                        var font = parseInt($(this).css("font-size")) - 5;
                        $(this).css("font-size",font + "px");
                    }

                var containerHeight = $(this).parent().height();
                var textHeight = $(this).height();
                if (textHeight > 0)
                {
                    var top = (containerHeight / 2) - (textHeight / 2);
                    $(this).css("top", top);
                }
  

            });

        },

        List: function (target, data, rightColumnDataType) {
            var output = "";

            if (rightColumnDataType.length > 0) {
                if (rightColumnDataType == "number") {
                    output = "<ul class='panel-list number-right'>";
                } else if (rightColumnDataType == "string") {
                    output = "<ul class='panel-list string-right'>";
                }
            } else {
                output = "<ul class='panel-list'>";
            }

            for (var i = 0; i < data.length; i++) {
                output += "<li>" + data[i] + "</li>";
            }

            output += "</ul>";
            this.Write(target, output);
        },

        CleanDateTime: function (dt) {
            return new Date(parseInt(dt.replace(/[^0-9 +]/g, '')));
        }

    }

})();

Date.prototype.monthNames = [
    "January", "February", "March",
    "April", "May", "June",
    "July", "August", "September",
    "October", "November", "December"
];
Date.prototype.dayNames = [
    "Sunday", "Monday", "Tuesday",
    "Wednesday", "Thursday", "Friday",
    "Saturday"
];
Date.prototype.getMonthName = function () {
    return this.monthNames[this.getMonth()];
};
Date.prototype.getShortMonthName = function () {
    return this.getMonthName().substr(0, 3);
};
Date.prototype.getDayName = function () {
    return this.dayNames[this.getDay()];
};
Date.prototype.getShortDayName = function () {
    return this.getDayName().substr(0, 3);
};
Date.prototype.getShortYear = function () {
    return this.getFullYear().toString().substr(2, 2);
};
Date.prototype.toFormattedString = function () {
    return this.getDayName() + ", " + this.getMonthName() + " " + this.getDate().getOrdinal() + " " + this.getFullYear();
};
Date.prototype.toShortFormattedString = function () {
    return this.getShortMonthName().substr(0, 3) + " '" + this.getShortYear();
};
Number.prototype.getOrdinal = function () {
    if (this % 1) return this;
    var s = this % 100;
    if (s > 3 && s < 21) return this + 'th';
    switch (s % 10) {
        case 1: return this + 'st';
        case 2: return this + 'nd';
        case 3: return this + 'rd';
        default: return this + 'th';
    }
}