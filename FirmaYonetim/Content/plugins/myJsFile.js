var companySelect = document.querySelector(".companySelect");
var addressSelect = document.querySelector(".addressSelect");
var selectedOption = document.querySelector(".selectedOption");
var contactSelect = document.querySelector(".contactSelect");
companySelect.addEventListener("change", () => {
    if (companySelect.value) {
        fetch("/Address/FullAddressAPI/" + companySelect.value)
            .then(response => response.text())
            .then(data => {
                var datas = JSON.parse(data);
                var Options = document.querySelectorAll(".addOption");
                for (var i = 0; i < Options.length; i++) {
                    Options[i].remove();
                }
                var addOptionContacts = document.querySelectorAll(".addOptionContact");
                for (var i = 0; i < addOptionContacts.length; i++) {
                    addOptionContacts[i].remove();
                }
                contactSelect.setAttribute("disabled", "");
                for (var i = 0; i < datas.length; i++) {
                    console.log(datas[i]);
                    var option = document.createElement("option");
                    option.value = datas[i].Id;
                    option.innerText = datas[i].Name;
                    option.classList = "addOption";
                    addressSelect.appendChild(option);
                }
            });
        addressSelect.removeAttribute("disabled");
    } else {
        var Options = document.querySelectorAll(".addOption");
        for (var i = 0; i < Options.length; i++) {
            Options[i].remove();
        }
        var addOptionContacts = document.querySelectorAll(".addOptionContact");
        for (var i = 0; i < addOptionContacts.length; i++) {
            addOptionContacts[i].remove();
        }
        contactSelect.setAttribute("disabled", "");
        addressSelect.setAttribute("disabled", "");
    }
});
addressSelect.addEventListener("change", () => {
    if (addressSelect.value) {
        fetch("/Address/FullContactAPI/" + addressSelect.value)
            .then(response => response.text())
            .then(data => {
                var datas = JSON.parse(data);
                var Options = document.querySelectorAll(".addOptionContact");
                for (var i = 0; i < Options.length; i++) {
                    Options[i].remove();
                }
                for (var i = 0; i < datas.length; i++) {
                    console.log(datas[i]);
                    var option = document.createElement("option");
                    option.value = datas[i].Id;
                    option.innerText = datas[i].Name + " " + datas[i].Surname;
                    option.classList = "addOptionContact";
                    contactSelect.appendChild(option);
                }
            });
        contactSelect.removeAttribute("disabled");
    } else {
        var Options = document.querySelectorAll(".addOptionContact");
        for (var i = 0; i < Options.length; i++) {
            Options[i].remove();
        }
        contactSelect.setAttribute("disabled", "");
    }
});