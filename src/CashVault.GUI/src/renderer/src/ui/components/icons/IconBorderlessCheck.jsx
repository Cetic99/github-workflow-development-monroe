/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconBorderlessCheck = ({ color = 'black', size = 'm' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="24"
        height="18"
        viewBox="0 0 24 18"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M23.1213 0.87868C24.2929 2.05025 24.2929 3.94975 23.1213 5.12132L11.1213 17.1213C9.94975 18.2929 8.05025 18.2929 6.87868 17.1213L0.87868 11.1213C-0.292893 9.94975 -0.292893 8.05025 0.87868 6.87868C2.05025 5.70711 3.94975 5.70711 5.12132 6.87868L9 10.7574L18.8787 0.87868C20.0503 -0.292893 21.9497 -0.292893 23.1213 0.87868Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconBorderlessCheck
