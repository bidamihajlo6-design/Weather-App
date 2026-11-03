const cards = document.querySelectorAll(".forecast-card");
console.log("Знайдено карток:", cards.length);

cards.forEach((card) => {
    console.log("Додаємо обробник");

    card.addEventListener("click", () => {
        const details = card.querySelector(".forecast-card-details");

        const isOpen = details.classList.contains("expanded");
        cards.forEach(otherCard =>{
            const otherDetails = otherCard.querySelector(".forecast-card-details");

            otherDetails.classList.remove("expanded");
            otherCard.classList.remove("expanded-card");
        });
        if (!isOpen) {
            details.classList.add("expanded");
            card.classList.add("expanded-card");
        }
    }); 
});

