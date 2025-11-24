const API_BASE_URL = "/api";

const eventsList = document.getElementById("eventsList");
const ticketsList = document.getElementById("ticketsList");
const eventModal = document.getElementById("eventModal");
const ticketModal = document.getElementById("ticketModal");
const createEventBtn = document.getElementById("createEventBtn");
const createTicketBtn = document.getElementById("createTicketBtn");
const eventForm = document.getElementById("eventForm");
const ticketForm = document.getElementById("ticketForm");

// Initialize app
document.addEventListener("DOMContentLoaded", () => {
  loadEvents();
  loadTickets();
  setupEventListeners();
});

// Setup Event Listeners
function setupEventListeners() {
  // Open modals
  createEventBtn.addEventListener("click", () => openModal(eventModal));
  createTicketBtn.addEventListener("click", () => {
    loadEventsForDropdown();
    openModal(ticketModal);
  });

  // Close modals
  document.querySelectorAll(".close").forEach((closeBtn) => {
    closeBtn.addEventListener("click", () => {
      closeModal(eventModal);
      closeModal(ticketModal);
    });
  });

  // Close modal on outside click
  window.addEventListener("click", (e) => {
    if (e.target === eventModal) closeModal(eventModal);
    if (e.target === ticketModal) closeModal(ticketModal);
  });

  // Form submissions
  eventForm.addEventListener("submit", handleEventSubmit);
  ticketForm.addEventListener("submit", handleTicketSubmit);
}

// Modal functions
function openModal(modal) {
  modal.style.display = "block";
}

function closeModal(modal) {
  modal.style.display = "none";
}

// Load Events
async function loadEvents() {
  try {
    const response = await fetch(`${API_BASE_URL}/events`);
    if (!response.ok) throw new Error("Failed to fetch events");

    const events = await response.json();
    displayEvents(events);
  } catch (error) {
    eventsList.innerHTML =
      `<div class="bg-red-100 border border-red-400 text-red-800 px-4 py-3 rounded-lg">Error loading events: ${error.message}</div>`;
  }
}

// Display Events
function displayEvents(events) {
  if (events.length === 0) {
    eventsList.innerHTML =
      '<p class="text-center text-gray-500 py-10">No events found. Create one!</p>';
    return;
  }

  eventsList.innerHTML = events.map((event) => `
        <div class="bg-gray-50 border border-gray-200 rounded-lg p-5 transition-all hover:shadow-md hover:translate-x-1">
            <div class="flex justify-between items-start mb-3">
                <h3 class="text-xl font-bold text-gray-800">${
    escapeHtml(event.name)
  }</h3>
                <button class="bg-red-500 hover:bg-red-600 text-white font-semibold px-4 py-2 rounded-md text-sm transition" onclick="deleteEvent(${event.id})">Delete</button>
            </div>
            <div class="text-gray-600 leading-relaxed space-y-2">
                <p><strong class="text-gray-700">Date:</strong> ${
    formatDate(event.eventDate)
  }</p>
                <p><strong class="text-gray-700">Location:</strong> ${
    escapeHtml(event.location)
  }</p>
                <p><strong class="text-gray-700">Description:</strong> ${
    escapeHtml(event.description)
  }</p>
                <p><strong class="text-gray-700">Available Seats:</strong> ${event.availableSeats} / ${event.totalSeats}</p>
                <p><strong class="text-gray-700">Price:</strong> $${
    event.basePrice.toFixed(2)
  }</p>
                <span class="inline-block px-3 py-1 rounded-full text-sm font-semibold ${
    event.availableSeats > 0
      ? "bg-green-100 text-green-800"
      : "bg-yellow-100 text-yellow-800"
  }">
                    ${event.availableSeats > 0 ? "Available" : "Sold Out"}
                </span>
            </div>
        </div>
    `).join("");
}

// Load Tickets
async function loadTickets() {
  try {
    const response = await fetch(`${API_BASE_URL}/tickets`);
    if (!response.ok) throw new Error("Failed to fetch tickets");

    const tickets = await response.json();
    displayTickets(tickets);
  } catch (error) {
    ticketsList.innerHTML =
      `<div class="bg-red-100 border border-red-400 text-red-800 px-4 py-3 rounded-lg">Error loading tickets: ${error.message}</div>`;
  }
}

// Display Tickets
function displayTickets(tickets) {
  if (tickets.length === 0) {
    ticketsList.innerHTML =
      '<p class="text-center text-gray-500 py-10">No tickets purchased yet.</p>';
    return;
  }

  ticketsList.innerHTML = tickets.map((ticket) => `
        <div class="bg-gray-50 border border-gray-200 rounded-lg p-5 transition-all hover:shadow-md hover:translate-x-1">
            <div class="flex justify-between items-start mb-3">
                <h3 class="text-xl font-bold text-gray-800">${
    escapeHtml(ticket.attendeeFullName)
  }</h3>
                <button class="bg-red-500 hover:bg-red-600 text-white font-semibold px-4 py-2 rounded-md text-sm transition" onclick="deleteTicket(${ticket.id})">Cancel</button>
            </div>
            <div class="text-gray-600 leading-relaxed space-y-2">
                <p><strong class="text-gray-700">Event:</strong> ${
    ticket.event ? escapeHtml(ticket.event.name) : "N/A"
  }</p>
                <p><strong class="text-gray-700">Email:</strong> ${
    escapeHtml(ticket.attendeeEmail)
  }</p>
                <p><strong class="text-gray-700">Seat:</strong> ${
    escapeHtml(ticket.seatNumber)
  }</p>
                <p><strong class="text-gray-700">Price Paid:</strong> $${
    ticket.pricePaid.toFixed(2)
  }</p>
                <p><strong class="text-gray-700">Purchased:</strong> ${
    formatDate(ticket.purchasedAt)
  }</p>
                <span class="inline-block px-3 py-1 rounded-full text-sm font-semibold bg-blue-100 text-blue-800">Status: ${
    getStatusText(ticket.status)
  }</span>
            </div>
        </div>
    `).join("");
}

// Load Events for Dropdown
async function loadEventsForDropdown() {
  try {
    const response = await fetch(`${API_BASE_URL}/events`);
    if (!response.ok) throw new Error("Failed to fetch events");

    const events = await response.json();
    const select = document.getElementById("ticketEvent");

    select.innerHTML = events.length === 0
      ? '<option value="">No events available</option>'
      : '<option value="">Select an event</option>' +
        events.map((event) =>
          `<option value="${event.id}" data-price="${event.basePrice}">
                    ${escapeHtml(event.name)} - $${event.basePrice.toFixed(2)}
                </option>`
        ).join("");

    // Auto-fill price when event is selected
    select.addEventListener("change", (e) => {
      const selectedOption = e.target.options[e.target.selectedIndex];
      const price = selectedOption.dataset.price;
      if (price) {
        document.getElementById("pricePaid").value = price;
      }
    });
  } catch (error) {
    console.error("Error loading events for dropdown:", error);
  }
}

// Handle Event Form Submit
async function handleEventSubmit(e) {
  e.preventDefault();

  const eventData = {
    name: document.getElementById("eventName").value,
    description: document.getElementById("eventDescription").value,
    eventDate: document.getElementById("eventDate").value,
    location: document.getElementById("eventLocation").value,
    totalSeats: parseInt(document.getElementById("totalSeats").value),
    availableSeats: parseInt(document.getElementById("totalSeats").value),
    basePrice: parseFloat(document.getElementById("basePrice").value),
    createdAt: new Date().toISOString(),
    tickets: [],
  };

  try {
    const response = await fetch(`${API_BASE_URL}/events`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(eventData),
    });

    if (!response.ok) throw new Error("Failed to create event");

    closeModal(eventModal);
    eventForm.reset();
    loadEvents();
    showSuccess("Event created successfully!");
  } catch (error) {
    alert("Error creating event: " + error.message);
  }
}

// Handle Ticket Form Submit
async function handleTicketSubmit(e) {
  e.preventDefault();

  const ticketData = {
    eventId: parseInt(document.getElementById("ticketEvent").value),
    attendeeFullName: document.getElementById("attendeeName").value,
    attendeeEmail: document.getElementById("attendeeEmail").value,
    seatNumber: document.getElementById("seatNumber").value,
    pricePaid: parseFloat(document.getElementById("pricePaid").value),
    status: 1, // Paid
    purchasedAt: new Date().toISOString(),
  };

  try {
    const response = await fetch(`${API_BASE_URL}/tickets`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(ticketData),
    });

    if (!response.ok) throw new Error("Failed to purchase ticket");

    closeModal(ticketModal);
    ticketForm.reset();
    loadTickets();
    loadEvents(); // Refresh to update available seats
    showSuccess("Ticket purchased successfully!");
  } catch (error) {
    alert("Error purchasing ticket: " + error.message);
  }
}

// Delete Event
async function deleteEvent(id) {
  if (!confirm("Are you sure you want to delete this event?")) return;

  try {
    const response = await fetch(`${API_BASE_URL}/events/${id}`, {
      method: "DELETE",
    });

    if (!response.ok) throw new Error("Failed to delete event");

    loadEvents();
    loadTickets(); // Refresh tickets as they may be affected
    showSuccess("Event deleted successfully!");
  } catch (error) {
    alert("Error deleting event: " + error.message);
  }
}

// Delete Ticket
async function deleteTicket(id) {
  if (!confirm("Are you sure you want to cancel this ticket?")) return;

  try {
    const response = await fetch(`${API_BASE_URL}/tickets/${id}`, {
      method: "DELETE",
    });

    if (!response.ok) throw new Error("Failed to cancel ticket");

    loadTickets();
    loadEvents(); // Refresh to update available seats
    showSuccess("Ticket cancelled successfully!");
  } catch (error) {
    alert("Error cancelling ticket: " + error.message);
  }
}

// Utility Functions
function formatDate(dateString) {
  const date = new Date(dateString);
  return date.toLocaleString("en-US", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function getStatusText(status) {
  const statuses = {
    0: "Reserved",
    1: "Paid",
    2: "Cancelled",
    3: "Checked In",
  };
  return statuses[status] || "Unknown";
}

function escapeHtml(text) {
  const div = document.createElement("div");
  div.textContent = text;
  return div.innerHTML;
}

function showSuccess(message) {
  const successDiv = document.createElement("div");
  successDiv.className =
    "fixed top-4 right-4 bg-green-100 border border-green-400 text-green-800 px-6 py-4 rounded-lg shadow-lg z-50 animate-fade-in";
  successDiv.textContent = message;
  document.body.appendChild(successDiv);

  setTimeout(() => successDiv.remove(), 3000);
}
