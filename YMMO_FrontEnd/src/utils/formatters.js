/**
 * Formate un nombre en ajoutant un point comme séparateur de milliers
 * @param {number|string} value - La valeur brute à formater
 * @returns {string} - La valeur formatée (ex: 1.000.000)
 */
export const formatNumber = (value) => {
    if (!value) return '0';
    return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, " ");
};

/**
 * Nettoie une chaîne formatée pour récupérer uniquement les chiffres
 * @param {string} value - La valeur formatée (ex: 1.000.000)
 * @returns {number} - La valeur brute (ex: 1000000)
 */
export const parseNumber = (value) => {
    return parseInt(value.toString().replace(/\./g, '')) || 0;
};