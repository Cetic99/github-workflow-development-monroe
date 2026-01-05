/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconPerspective = ({ color = 'black', size = 'm' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="24"
        height="24"
        viewBox="0 0 24 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M9 5C9 4.44772 9.44772 4 10 4C10.5523 4 11 4.44772 11 5V11H14V7C14 6.44772 14.4477 6 15 6C15.5523 6 16 6.44772 16 7V11L20 11C20.5523 11 21 11.4477 21 12C21 12.5523 20.5523 13 20 13H16V17C16 17.5523 15.5523 18 15 18C14.4477 18 14 17.5523 14 17V13L11 13V19C11 19.5523 10.5523 20 10 20C9.44772 20 9 19.5523 9 19V13H4C3.44772 13 3 12.5523 3 12C3 11.4477 3.44772 11 4 11L9 11V5Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M3 5.98477C3 4.0696 4.77016 2.6448 6.64109 3.05407L18.6411 5.67907C20.0183 5.98034 21 7.19995 21 8.60977V15.3902C21 16.8 20.0183 18.0196 18.6411 18.3209L6.64109 20.9459C4.77016 21.3551 3 19.9304 3 18.0152V5.98477ZM6.2137 5.00787C5.59005 4.87145 5 5.34638 5 5.98477V18.0152C5 18.6536 5.59005 19.1285 6.2137 18.9921L18.2137 16.3671C18.6728 16.2667 19 15.8601 19 15.3902V8.60977C19 8.13983 18.6728 7.73329 18.2137 7.63287L6.2137 5.00787Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconPerspective
